using Microsoft.AspNetCore.Mvc;
using FinancialApp.Application.Interfaces;
using System.Text;
using System.Globalization;
using FinancialApp.Application.DTOs;

namespace FinancialApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportExportController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ICategoryService _categoryService;

    public ImportExportController(ITransactionService transactionService, ICategoryService categoryService)
    {
        _transactionService = transactionService;
        _categoryService = categoryService;
    }

    [HttpGet("export/{userId}")]
    public async Task<IActionResult> ExportTransactionsToCSV(int userId, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
    {
        var transactions = await _transactionService.GetUserTransactionsAsync(userId);
        
        // Filter by date if provided
        if (fromDate.HasValue)
            transactions = transactions.Where(t => t.TransactionDate >= fromDate.Value);
        if (toDate.HasValue)
            transactions = transactions.Where(t => t.TransactionDate <= toDate.Value);

        var csv = new StringBuilder();
        
        // Header
        csv.AppendLine("Ngày,Loại,Danh mục,Số tiền,Mô tả");
        
        // Data
        foreach (var transaction in transactions)
        {
            csv.AppendLine($"\"{transaction.TransactionDate:dd/MM/yyyy}\"," +
                          $"\"{transaction.Type}\"," +
                          $"\"{transaction.Category}\"," +
                          $"\"{transaction.Amount:N0}\"," +
                          $"\"{transaction.Description}\"");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        var fileName = $"transactions_{userId}_{DateTime.Now:yyyyMMdd}.csv";
        
        return File(bytes, "text/csv", fileName);
    }

    [HttpPost("import/{userId}")]
    public async Task<ActionResult<ImportResultDto>> ImportTransactionsFromCSV(int userId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File không hợp lệ");

        var result = new ImportResultDto();
        var categories = await _categoryService.GetActiveCategoriesAsync();
        var categoryNames = categories.Select(c => c.Name.ToLower()).ToHashSet();

        using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
        var line = await reader.ReadLineAsync(); // Skip header
        
        while ((line = await reader.ReadLineAsync()) != null)
        {
            try
            {
                var values = ParseCSVLine(line);
                if (values.Length < 5) continue;

                var transactionDate = DateTime.ParseExact(values[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var type = values[1].ToLower();
                var category = values[2];
                var amount = decimal.Parse(values[3].Replace(",", "").Replace(".", ""));
                var description = values[4];

                // Validate data
                if (type != "income" && type != "expense")
                {
                    result.Errors.Add($"Dòng {result.TotalRows + 1}: Loại giao dịch không hợp lệ '{type}'");
                    result.TotalRows++;
                    continue;
                }

                if (!categoryNames.Contains(category.ToLower()))
                {
                    result.Warnings.Add($"Dòng {result.TotalRows + 1}: Danh mục '{category}' chưa tồn tại, sẽ sử dụng 'Khác'");
                    category = "Khác";
                }

                var createDto = new CreateTransactionDto
                {
                    Type = type,
                    Category = category,
                    Amount = amount,
                    Description = description,
                    TransactionDate = transactionDate
                };

                await _transactionService.CreateTransactionAsync(userId, createDto);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Dòng {result.TotalRows + 1}: {ex.Message}");
            }
            
            result.TotalRows++;
        }

        return Ok(result);
    }

    [HttpGet("template")]
    public IActionResult DownloadCSVTemplate()
    {
        var csv = new StringBuilder();
        csv.AppendLine("Ngày,Loại,Danh mục,Số tiền,Mô tả");
        csv.AppendLine("\"26/09/2025\",\"expense\",\"Đi chợ\",\"50000\",\"Mua rau củ\"");
        csv.AppendLine("\"25/09/2025\",\"income\",\"Lương\",\"5000000\",\"Lương tháng 9\"");

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", "transaction_template.csv");
    }

    private string[] ParseCSVLine(string line)
    {
        var result = new List<string>();
        var inQuotes = false;
        var current = new StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString().Trim());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        result.Add(current.ToString().Trim());
        return result.ToArray();
    }
}

public class ImportResultDto
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public List<string> Warnings { get; set; } = new List<string>();
}