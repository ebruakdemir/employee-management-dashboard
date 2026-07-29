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

    public async Task<List<EmployeeResponseDto>> GetAllAsync()
    {
        List<Employee> employees =
            await _context.Employees.ToListAsync();

        return _mapper.Map<List<EmployeeResponseDto>>(employees);
    }

    public Task<EmployeeResponseDto?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<EmployeeResponseDto?> CreateAsync(CreateEmployeeDto createEmployeeDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EmailExistsAsync(string email, int? excludedEmployeeId = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(int id, UpdateEmployeeDto updateEmployeeDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}