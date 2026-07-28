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
}