using EmployeeManagement.Api.DTOs;

namespace EmployeeManagement.Api.Interfaces;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeResponseDto>> GetEmployeesAsync(
        EmployeeQueryParameters queryParameters
    );

    Task<EmployeeResponseDto?> GetEmployeeByIdAsync(
        int id
    );

    Task<EmployeeResponseDto> CreateEmployeeAsync(
        CreateEmployeeDto createEmployeeDto
    );

    Task<EmployeeUpdateResult> UpdateEmployeeAsync(
        int id,
        UpdateEmployeeDto updateEmployeeDto
    );

    Task<bool> DeleteEmployeeAsync(
        int id
    );

    Task<EmployeeStatisticsDto> GetStatisticsAsync();
}