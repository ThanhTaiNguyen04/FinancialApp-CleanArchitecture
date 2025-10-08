using FinancialApp.Application.DTOs;
using FinancialApp.Application.Interfaces;
using FinancialApp.Domain.Entities;
using FinancialApp.Domain.Interfaces;

namespace FinancialApp.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(MapToCategoryDto);
    }

    public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
    {
        var categories = await _categoryRepository.GetActiveAsync();
        return categories.Select(MapToCategoryDto);
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesByTypeAsync(string type)
    {
        var categories = await _categoryRepository.GetByTypeAsync(type);
        return categories.Select(MapToCategoryDto);
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category != null ? MapToCategoryDto(category) : null;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        // Check if category with same name already exists
        var existingCategory = await _categoryRepository.GetByNameAsync(createCategoryDto.Name);
        if (existingCategory != null)
        {
            throw new InvalidOperationException("Category with this name already exists");
        }

        var category = new Category
        {
            Name = createCategoryDto.Name,
            IconName = createCategoryDto.IconName,
            ColorCode = createCategoryDto.ColorCode,
            Type = createCategoryDto.Type,
            IsActive = true
        };

        var createdCategory = await _categoryRepository.AddAsync(category);
        return MapToCategoryDto(createdCategory);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(int id, CreateCategoryDto updateCategoryDto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            throw new ArgumentException("Category not found");
        }

        // Check if another category with same name exists
        var existingCategory = await _categoryRepository.GetByNameAsync(updateCategoryDto.Name);
        if (existingCategory != null && existingCategory.Id != id)
        {
            throw new InvalidOperationException("Category with this name already exists");
        }

        category.Name = updateCategoryDto.Name;
        category.IconName = updateCategoryDto.IconName;
        category.ColorCode = updateCategoryDto.ColorCode;
        category.Type = updateCategoryDto.Type;

        var updatedCategory = await _categoryRepository.UpdateAsync(category);
        return MapToCategoryDto(updatedCategory);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            throw new ArgumentException("Category not found");
        }

        // Instead of hard delete, soft delete by setting IsActive to false
        category.IsActive = false;
        await _categoryRepository.UpdateAsync(category);
    }

    private CategoryDto MapToCategoryDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            IconName = category.IconName,
            ColorCode = category.ColorCode,
            Type = category.Type,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };
    }
}