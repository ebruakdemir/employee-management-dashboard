using AutoMapper;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Interfaces;
using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IEmployeeService _employeeService;

    public EmployeesController(
        AppDbContext context,
        IMapper mapper,
        IEmployeeService employeeService)
    {
        _context = context;
        _mapper = mapper;
        _employeeService = employeeService;
    }

    // GET: api/employees
    [HttpGet]
    public async Task<ActionResult<List<EmployeeResponseDto>>> GetEmployees()
    {
        List<EmployeeResponseDto> employees =
            await _employeeService.GetAllAsync();

        return Ok(employees);
    }

    // GET: api/employees/5
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeResponseDto>> GetEmployee(int id)
    {
        Employee? employee =
            await _context.Employees.FindAsync(id);

        if (employee is null)
        {
            return NotFound();
        }

        EmployeeResponseDto response =
            _mapper.Map<EmployeeResponseDto>(employee);

        return Ok(response);
    }

    // POST: api/employees
    [HttpPost]
    public async Task<ActionResult<EmployeeResponseDto>> CreateEmployee(
        CreateEmployeeDto createEmployeeDto)
    {
        bool emailExists = await _context.Employees.AnyAsync(
            employee => employee.Email == createEmployeeDto.Email);

        if (emailExists)
        {
            return Conflict(new
            {
                message = "An employee with this email already exists."
            });
        }

        Employee employee =
            _mapper.Map<Employee>(createEmployeeDto);

        _context.Employees.Add(employee);

        await _context.SaveChangesAsync();

        EmployeeResponseDto response =
            _mapper.Map<EmployeeResponseDto>(employee);

        return CreatedAtAction(
            nameof(GetEmployee),
            new { id = employee.Id },
            response);
    }

    // PUT: api/employees/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(
        int id,
        UpdateEmployeeDto updateEmployeeDto)
    {
        Employee? employee =
            await _context.Employees.FindAsync(id);

        if (employee is null)
        {
            return NotFound();
        }

        bool emailExists = await _context.Employees.AnyAsync(
            existingEmployee =>
                existingEmployee.Email == updateEmployeeDto.Email &&
                existingEmployee.Id != id);

        if (emailExists)
        {
            return Conflict(new
            {
                message = "An employee with this email already exists."
            });
        }

        _mapper.Map(updateEmployeeDto, employee);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/employees/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        Employee? employee =
            await _context.Employees.FindAsync(id);

        if (employee is null)
        {
            return NotFound();
        }

        _context.Employees.Remove(employee);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}