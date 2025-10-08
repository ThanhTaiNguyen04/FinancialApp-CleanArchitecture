using FinancialApp.Application.DTOs;

namespace FinancialApp.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();
    Task<IEnumerable<CategoryDto>> GetCategoriesByTypeAsync(string type);
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
    Task<CategoryDto> UpdateCategoryAsync(int id, CreateCategoryDto updateCategoryDto);
    Task DeleteCategoryAsync(int id);
}