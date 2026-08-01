using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Interfaces;
using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;

    public EmployeeService(
        AppDbContext context
    )
    {
        _context = context;
    }

    public async Task<PagedResult<EmployeeResponseDto>>
        GetEmployeesAsync(
            EmployeeQueryParameters queryParameters
        )
    {
        IQueryable<Employee> query =
            _context.Employees.AsNoTracking();

        if (
            !string.IsNullOrWhiteSpace(
                queryParameters.Search
            )
        )
        {
            var search =
                queryParameters.Search
                    .Trim()
                    .ToLower();

            query = query.Where(employee =>
                employee.FirstName
                    .ToLower()
                    .Contains(search) ||

                employee.LastName
                    .ToLower()
                    .Contains(search) ||

                employee.Email
                    .ToLower()
                    .Contains(search) ||

                employee.Department
                    .ToLower()
                    .Contains(search) ||

                employee.Position
                    .ToLower()
                    .Contains(search)
            );
        }

        if (
            !string.IsNullOrWhiteSpace(
                queryParameters.Department
            )
        )
        {
            var department =
                queryParameters.Department.Trim();

            query = query.Where(employee =>
                employee.Department == department
            );
        }

        query = ApplySorting(
            query,
            queryParameters.Sort
        );

        var totalCount =
            await query.CountAsync();

        var page =
            queryParameters.Page < 1
                ? 1
                : queryParameters.Page;

        var pageSize =
            queryParameters.PageSize < 1
                ? 10
                : Math.Min(
                    queryParameters.PageSize,
                    100
                );

        var items =
            await query
                .Skip(
                    (page - 1) * pageSize
                )
                .Take(pageSize)
                .Select(employee =>
                    ToResponseDto(employee)
                )
                .ToListAsync();

        return new PagedResult<EmployeeResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages =
                totalCount == 0
                    ? 0
                    : (int)Math.Ceiling(
                        totalCount /
                        (double)pageSize
                    )
        };
    }

    public async Task<EmployeeResponseDto?>
        GetEmployeeByIdAsync(
            int id
        )
    {
        var employee =
            await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    employee =>
                        employee.Id == id
                );

        return employee is null
            ? null
            : ToResponseDto(employee);
    }

    public async Task<EmployeeResponseDto>
        CreateEmployeeAsync(
            CreateEmployeeDto createEmployeeDto
        )
    {
        var emailExists =
            await _context.Employees
                .AnyAsync(employee =>
                    employee.Email ==
                    createEmployeeDto.Email
                );

        if (emailExists)
        {
            throw new InvalidOperationException(
                "An employee with this email already exists."
            );
        }

        var employee = new Employee
        {
            FirstName =
                createEmployeeDto.FirstName.Trim(),

            LastName =
                createEmployeeDto.LastName.Trim(),

            Email =
                createEmployeeDto.Email
                    .Trim()
                    .ToLower(),

            Department =
                createEmployeeDto.Department.Trim(),

            Position =
                createEmployeeDto.Position.Trim(),

            Salary =
                createEmployeeDto.Salary,

            HireDate =
                createEmployeeDto.HireDate,

            IsActive =
                createEmployeeDto.IsActive
        };

        _context.Employees.Add(employee);

        await _context.SaveChangesAsync();

        return ToResponseDto(employee);
    }

    public async Task<EmployeeUpdateResult>
        UpdateEmployeeAsync(
            int id,
            UpdateEmployeeDto updateEmployeeDto
        )
    {
        var employee =
            await _context.Employees
                .FirstOrDefaultAsync(
                    employee =>
                        employee.Id == id
                );

        if (employee is null)
        {
            return EmployeeUpdateResult.NotFound;
        }

        var normalizedEmail =
            updateEmployeeDto.Email
                .Trim()
                .ToLower();

        var emailExists =
            await _context.Employees
                .AnyAsync(otherEmployee =>
                    otherEmployee.Id != id &&
                    otherEmployee.Email ==
                    normalizedEmail
                );

        if (emailExists)
        {
            return EmployeeUpdateResult.EmailConflict;
        }

        employee.FirstName =
            updateEmployeeDto.FirstName.Trim();

        employee.LastName =
            updateEmployeeDto.LastName.Trim();

        employee.Email =
            normalizedEmail;

        employee.Department =
            updateEmployeeDto.Department.Trim();

        employee.Position =
            updateEmployeeDto.Position.Trim();

        employee.Salary =
            updateEmployeeDto.Salary;

        employee.HireDate =
            updateEmployeeDto.HireDate;

        employee.IsActive =
            updateEmployeeDto.IsActive;

        await _context.SaveChangesAsync();

        return EmployeeUpdateResult.Success;
    }

    public async Task<bool> DeleteEmployeeAsync(
        int id
    )
    {
        var employee =
            await _context.Employees
                .FirstOrDefaultAsync(
                    employee =>
                        employee.Id == id
                );

        if (employee is null)
        {
            return false;
        }

        _context.Employees.Remove(employee);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<EmployeeStatisticsDto>
        GetStatisticsAsync()
    {
        var totalEmployees =
            await _context.Employees
                .CountAsync();

        var activeEmployees =
            await _context.Employees
                .CountAsync(employee =>
                    employee.IsActive
                );

        var departmentCount =
            await _context.Employees
                .Where(employee =>
                    employee.Department != null &&
                    employee.Department != ""
                )
                .Select(employee =>
                    employee.Department
                )
                .Distinct()
                .CountAsync();

        decimal averageSalary = 0;

        if (totalEmployees > 0)
        {
            averageSalary =
                await _context.Employees
                    .AverageAsync(employee =>
                        employee.Salary
                    );
        }

        return new EmployeeStatisticsDto
        {
            TotalEmployees =
                totalEmployees,

            ActiveEmployees =
                activeEmployees,

            DepartmentCount =
                departmentCount,

            AverageSalary =
                averageSalary
        };
    }

    private static IQueryable<Employee>
        ApplySorting(
            IQueryable<Employee> query,
            string? sort
        )
    {
        return sort?.ToLower() switch
        {
            "name_desc" =>
                query
                    .OrderByDescending(employee =>
                        employee.FirstName
                    )
                    .ThenByDescending(employee =>
                        employee.LastName
                    ),

            "department" =>
                query.OrderBy(employee =>
                    employee.Department
                ),

            "department_desc" =>
                query.OrderByDescending(employee =>
                    employee.Department
                ),

            "hiredate" =>
                query.OrderBy(employee =>
                    employee.HireDate
                ),

            "hiredate_desc" =>
                query.OrderByDescending(employee =>
                    employee.HireDate
                ),

            "salary" =>
                query.OrderBy(employee =>
                    employee.Salary
                ),

            "salary_desc" =>
                query.OrderByDescending(employee =>
                    employee.Salary
                ),

            _ =>
                query
                    .OrderBy(employee =>
                        employee.FirstName
                    )
                    .ThenBy(employee =>
                        employee.LastName
                    )
        };
    }

    private static EmployeeResponseDto
        ToResponseDto(
            Employee employee
        )
    {
        return new EmployeeResponseDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Department = employee.Department,
            Position = employee.Position,
            Salary = employee.Salary,
            HireDate = employee.HireDate,
            IsActive = employee.IsActive
        };
    }
}