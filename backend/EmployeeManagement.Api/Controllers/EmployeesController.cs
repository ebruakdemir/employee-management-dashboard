using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmployeesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Employee>>> GetEmployees()
    {
        List<Employee> employees =
            await _context.Employees.ToListAsync();

        return Ok(employees);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Employee>> GetEmployeeById(int id)
    {
        Employee? employee =
            await _context.Employees.FindAsync(id);

        if (employee is null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<Employee>> CreateEmployee(
        Employee employee)
    {
        bool emailExists = await _context.Employees
            .AnyAsync(e => e.Email == employee.Email);

        if (emailExists)
        {
            return Conflict(new
            {
                message = "An employee with this email address already exists."
            });
        }

        _context.Employees.Add(employee);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEmployeeById),
            new { id = employee.Id },
            employee
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateEmployee(
        int id,
        Employee updatedEmployee)
    {
        Employee? existingEmployee =
            await _context.Employees.FindAsync(id);

        if (existingEmployee is null)
        {
            return NotFound();
        }

        bool emailExists = await _context.Employees
            .AnyAsync(e =>
                e.Email == updatedEmployee.Email &&
                e.Id != id);

        if (emailExists)
        {
            return Conflict(new
            {
                message = "An employee with this email address already exists."
            });
        }

        existingEmployee.FirstName = updatedEmployee.FirstName;
        existingEmployee.LastName = updatedEmployee.LastName;
        existingEmployee.Email = updatedEmployee.Email;
        existingEmployee.Department = updatedEmployee.Department;
        existingEmployee.Position = updatedEmployee.Position;
        existingEmployee.Salary = updatedEmployee.Salary;
        existingEmployee.HireDate = updatedEmployee.HireDate;
        existingEmployee.IsActive = updatedEmployee.IsActive;

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
            return NotFound();
        }

        _context.Employees.Remove(employee);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}