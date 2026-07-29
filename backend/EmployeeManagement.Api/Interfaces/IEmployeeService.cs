using EmployeeManagement.Api.DTOs;

namespace EmployeeManagement.Api.Interfaces;

public interface IEmployeeService
{
    Task<List<EmployeeResponseDto>> GetAllAsync();

    Task<EmployeeResponseDto?> GetByIdAsync(int id);

    Task<EmployeeResponseDto?> CreateAsync(
        CreateEmployeeDto createEmployeeDto);

    Task<bool> EmailExistsAsync(
        string email,
        int? excludedEmployeeId = null);

    Task<bool> UpdateAsync(
        int id,
        UpdateEmployeeDto updateEmployeeDto);

    Task<bool> DeleteAsync(int id);
}