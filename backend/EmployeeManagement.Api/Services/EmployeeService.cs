using AutoMapper;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Interfaces;
using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public EmployeeService(
        AppDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<EmployeeResponseDto>> GetAllAsync(
        EmployeeQueryParameters queryParameters)
    {
        IQueryable<Employee> query =
            _context.Employees.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(queryParameters.Search))
        {
            string search = queryParameters.Search.Trim();

            query = query.Where(employee =>
                employee.FirstName.Contains(search) ||
                employee.LastName.Contains(search) ||
                employee.Email.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(queryParameters.Department))
        {
            string department =
                queryParameters.Department.Trim();

            query = query.Where(employee =>
                employee.Department == department);
        }

        query = queryParameters.Sort?.ToLower() switch
        {
            "name" =>
                query.OrderBy(employee => employee.FirstName)
                     .ThenBy(employee => employee.LastName),

            "name_desc" =>
                query.OrderByDescending(employee => employee.FirstName)
                     .ThenByDescending(employee => employee.LastName),

            "salary" =>
                query.OrderBy(employee => employee.Salary),

            "salary_desc" =>
                query.OrderByDescending(employee => employee.Salary),

            "hiredate" =>
                query.OrderBy(employee => employee.HireDate),

            "hiredate_desc" =>
                query.OrderByDescending(employee => employee.HireDate),

            _ =>
                query.OrderBy(employee => employee.Id)
        };

        int page =
            queryParameters.Page < 1
                ? 1
                : queryParameters.Page;

        int pageSize =
            queryParameters.PageSize switch
            {
                < 1 => 10,
                > 100 => 100,
                _ => queryParameters.PageSize
            };

        int totalCount =
            await query.CountAsync();

        List<Employee> employees =
            await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        List<EmployeeResponseDto> employeeDtos =
            _mapper.Map<List<EmployeeResponseDto>>(employees);

        int totalPages =
            (int)Math.Ceiling(
                totalCount / (double)pageSize);

        return new PagedResult<EmployeeResponseDto>
        {
            Items = employeeDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public async Task<EmployeeResponseDto?> GetByIdAsync(int id)
    {
        Employee? employee =
            await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(employee =>
                    employee.Id == id);

        if (employee is null)
        {
            return null;
        }

        return _mapper.Map<EmployeeResponseDto>(employee);
    }

    public async Task<EmployeeResponseDto?> CreateAsync(
        CreateEmployeeDto createEmployeeDto)
    {
        bool emailExists =
            await EmailExistsAsync(createEmployeeDto.Email);

        if (emailExists)
        {
            return null;
        }

        Employee employee =
            _mapper.Map<Employee>(createEmployeeDto);

        _context.Employees.Add(employee);

        await _context.SaveChangesAsync();

        return _mapper.Map<EmployeeResponseDto>(employee);
    }

    public async Task<bool> EmailExistsAsync(
        string email,
        int? excludedEmployeeId = null)
    {
        return await _context.Employees.AnyAsync(employee =>
            employee.Email == email &&
            (!excludedEmployeeId.HasValue ||
             employee.Id != excludedEmployeeId.Value));
    }

    public async Task<EmployeeUpdateResult> UpdateAsync(
        int id,
        UpdateEmployeeDto updateEmployeeDto)
    {
        Employee? employee =
            await _context.Employees.FindAsync(id);

        if (employee is null)
        {
            return new EmployeeUpdateResult
            {
                EmployeeNotFound = true
            };
        }

        bool emailExists =
            await EmailExistsAsync(
                updateEmployeeDto.Email,
                id);

        if (emailExists)
        {
            return new EmployeeUpdateResult
            {
                EmailAlreadyExists = true
            };
        }

        _mapper.Map(updateEmployeeDto, employee);

        await _context.SaveChangesAsync();

        return new EmployeeUpdateResult
        {
            IsSuccessful = true
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Employee? employee =
            await _context.Employees.FindAsync(id);

        if (employee is null)
        {
            return false;
        }

        _context.Employees.Remove(employee);

        await _context.SaveChangesAsync();

        return true;
    }
}