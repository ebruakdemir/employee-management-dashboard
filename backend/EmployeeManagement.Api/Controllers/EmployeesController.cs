using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private static readonly List<Employee> Employees =
    [
        new Employee
        {
            Id = 1,
            FirstName = "Ebru",
            LastName = "Akdemir",
            Email = "ebru@example.com",
            Department = "Software",
            Position = "Junior Developer",
            Salary = 35000,
            HireDate = new DateTime(2026, 7, 1),
            IsActive = true
        },
        new Employee
        {
            Id = 2,
            FirstName = "Daniel",
            LastName = "Meyer",
            Email = "daniel@example.com",
            Department = "Human Resources",
            Position = "HR Specialist",
            Salary = 42000,
            HireDate = new DateTime(2025, 11, 15),
            IsActive = true
        }
    ];

    [HttpGet]
    public ActionResult<List<Employee>> GetEmployees()
    {
        return Ok(Employees);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Employee> GetEmployeeById(int id)
    {
        Employee? employee = Employees.FirstOrDefault(e => e.Id == id);

        if (employee is null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    [HttpPost]
    public ActionResult<Employee> CreateEmployee(Employee employee)
    {
        int nextId = Employees.Count == 0
            ? 1
            : Employees.Max(e => e.Id) + 1;

        employee.Id = nextId;
        Employees.Add(employee);

        return CreatedAtAction(
            nameof(GetEmployeeById),
            new { id = employee.Id },
            employee
        );
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateEmployee(int id, Employee updatedEmployee)
    {
        Employee? existingEmployee =
            Employees.FirstOrDefault(e => e.Id == id);

        if (existingEmployee is null)
        {
            return NotFound();
        }

        existingEmployee.FirstName = updatedEmployee.FirstName;
        existingEmployee.LastName = updatedEmployee.LastName;
        existingEmployee.Email = updatedEmployee.Email;
        existingEmployee.Department = updatedEmployee.Department;
        existingEmployee.Position = updatedEmployee.Position;
        existingEmployee.Salary = updatedEmployee.Salary;
        existingEmployee.HireDate = updatedEmployee.HireDate;
        existingEmployee.IsActive = updatedEmployee.IsActive;

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteEmployee(int id)
    {
        Employee? employee = Employees.FirstOrDefault(e => e.Id == id);

        if (employee is null)
        {
            return NotFound();
        }

        Employees.Remove(employee);

        return NoContent();
    }
}