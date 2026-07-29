using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(
        IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // GET: api/employees
    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeResponseDto>>>
        GetEmployees(
            [FromQuery] EmployeeQueryParameters queryParameters)
    {
        PagedResult<EmployeeResponseDto> result =
            await _employeeService.GetAllAsync(queryParameters);

        return Ok(result);
    }

    // GET: api/employees/5
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeResponseDto>> GetEmployee(int id)
    {
        EmployeeResponseDto? employee =
            await _employeeService.GetByIdAsync(id);

        if (employee is null)
        {
            return NotFound(new
            {
                message = $"Employee with id {id} was not found."
            });
        }

        return Ok(employee);
    }

    // POST: api/employees
    [HttpPost]
    public async Task<ActionResult<EmployeeResponseDto>> CreateEmployee(
        CreateEmployeeDto createEmployeeDto)
    {
        EmployeeResponseDto? createdEmployee =
            await _employeeService.CreateAsync(createEmployeeDto);

        if (createdEmployee is null)
        {
            return Conflict(new
            {
                message = "An employee with this email already exists."
            });
        }

        return CreatedAtAction(
            nameof(GetEmployee),
            new { id = createdEmployee.Id },
            createdEmployee);
    }

    // PUT: api/employees/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(
        int id,
        UpdateEmployeeDto updateEmployeeDto)
    {
        EmployeeUpdateResult result =
            await _employeeService.UpdateAsync(
                id,
                updateEmployeeDto);

        if (result.EmployeeNotFound)
        {
            return NotFound(new
            {
                message = $"Employee with id {id} was not found."
            });
        }

        if (result.EmailAlreadyExists)
        {
            return Conflict(new
            {
                message = "An employee with this email already exists."
            });
        }

        return NoContent();
    }

    // DELETE: api/employees/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        bool deleted =
            await _employeeService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = $"Employee with id {id} was not found."
            });
        }

        return NoContent();
    }
}