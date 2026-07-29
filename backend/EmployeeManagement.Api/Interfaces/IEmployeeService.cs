using EmployeeManagement.Api.DTOs;

namespace EmployeeManagement.Api.Interfaces;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeResponseDto>> GetAllAsync(
        EmployeeQueryParameters queryParameters);

    Task<EmployeeResponseDto?> GetByIdAsync(int id);

    Task<EmployeeResponseDto?> CreateAsync(
        CreateEmployeeDto createEmployeeDto);

    Task<bool> EmailExistsAsync(
        string email,
        int? excludedEmployeeId = null);

    Task<EmployeeUpdateResult> UpdateAsync(
        int id,
        UpdateEmployeeDto updateEmployeeDto);

    Task<bool> DeleteAsync(int id);
}