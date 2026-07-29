using AutoMapper;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public EmployeesController(
        AppDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmployeeResponseDto>>> GetEmployees()
    {
        List<Employee> employees =
            await _context.Employees.ToListAsync();

        List<EmployeeResponseDto> response =
            _mapper.Map<List<EmployeeResponseDto>>(employees);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeResponseDto>> GetEmployeeById(int id)
    {
        Employee? employee =
            await _context.Employees.FindAsync(id);

        if (employee is null)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        EmployeeResponseDto response =
            _mapper.Map<EmployeeResponseDto>(employee);

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeResponseDto>> CreateEmployee(
        CreateEmployeeDto createEmployeeDto)
    {
        bool emailExists = await _context.Employees
            .AnyAsync(employee =>
                employee.Email == createEmployeeDto.Email);

        if (emailExists)
        {
            return Conflict(new
            {
                message = "An employee with this email address already exists."
            });
        }

        Employee employee =
            _mapper.Map<Employee>(createEmployeeDto);

        _context.Employees.Add(employee);

        await _context.SaveChangesAsync();

        EmployeeResponseDto response =
            _mapper.Map<EmployeeResponseDto>(employee);

        return CreatedAtAction(
            nameof(GetEmployeeById),
            new { id = employee.Id },
            response
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateEmployee(
        int id,
        UpdateEmployeeDto updateEmployeeDto)
    {
        Employee? existingEmployee =
            await _context.Employees.FindAsync(id);

        if (existingEmployee is null)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        bool emailExists = await _context.Employees
            .AnyAsync(employee =>
                employee.Email == updateEmployeeDto.Email &&
                employee.Id != id);

        if (emailExists)
        {
            return Conflict(new
            {
                message = "An employee with this email address already exists."
            });
        }

        _mapper.Map(
            updateEmployeeDto,
            existingEmployee
        );

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        Employee? employee =
            await _context.Employees.FindAsync(id);

        if (employee is null)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        _context.Employees.Remove(employee);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}