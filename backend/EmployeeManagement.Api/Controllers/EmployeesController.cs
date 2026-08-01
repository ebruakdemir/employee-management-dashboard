using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(
        IEmployeeService employeeService
    )
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<
        ActionResult<PagedResult<EmployeeResponseDto>>
    > GetEmployees(
        [FromQuery]
        EmployeeQueryParameters queryParameters
    )
    {
        var result =
            await _employeeService.GetEmployeesAsync(
                queryParameters
            );

        return Ok(result);
    }

    /*
     * Bu endpoint, {id:int} endpointinden önce
     * veya sonra bulunabilir.
     *
     * Önemli olan aşağıdaki ID route'unun
     * {id:int} olmasıdır.
     */
    [HttpGet("statistics")]
    public async Task<
        ActionResult<EmployeeStatisticsDto>
    > GetStatistics()
    {
        var statistics =
            await _employeeService
                .GetStatisticsAsync();

        return Ok(statistics);
    }

    /*
     * :int kısıtlaması çok önemli.
     *
     * Böylece:
     * /api/Employees/4
     * bu metoda gelir.
     *
     * Ancak:
     * /api/Employees/statistics
     * bu metoda gelmez.
     */
    [HttpGet("{id:int}")]
    public async Task<
        ActionResult<EmployeeResponseDto>
    > GetEmployeeById(
        int id
    )
    {
        var employee =
            await _employeeService
                .GetEmployeeByIdAsync(id);

        if (employee is null)
        {
            return NotFound(
                new
                {
                    message =
                        "Employee could not be found."
                }
            );
        }

        return Ok(employee);
    }

    [HttpPost]
    public async Task<
        ActionResult<EmployeeResponseDto>
    > CreateEmployee(
        [FromBody]
        CreateEmployeeDto createEmployeeDto
    )
    {
        try
        {
            var createdEmployee =
                await _employeeService
                    .CreateEmployeeAsync(
                        createEmployeeDto
                    );

            return CreatedAtAction(
                nameof(GetEmployeeById),
                new
                {
                    id = createdEmployee.Id
                },
                createdEmployee
            );
        }
        catch (
            InvalidOperationException exception
        )
        {
            return Conflict(
                new
                {
                    message =
                        exception.Message
                }
            );
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult>
        UpdateEmployee(
            int id,
            [FromBody]
            UpdateEmployeeDto updateEmployeeDto
        )
    {
        var result =
            await _employeeService
                .UpdateEmployeeAsync(
                    id,
                    updateEmployeeDto
                );

        return result switch
        {
            EmployeeUpdateResult.Success =>
                NoContent(),

            EmployeeUpdateResult.NotFound =>
                NotFound(
                    new
                    {
                        message =
                            "Employee could not be found."
                    }
                ),

            EmployeeUpdateResult.EmailConflict =>
                Conflict(
                    new
                    {
                        message =
                            "An employee with this email already exists."
                    }
                ),

            _ =>
                StatusCode(
                    StatusCodes
                        .Status500InternalServerError
                )
        };
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
        DeleteEmployee(
            int id
        )
    {
        var deleted =
            await _employeeService
                .DeleteEmployeeAsync(id);

        if (!deleted)
        {
            return NotFound(
                new
                {
                    message =
                        "Employee could not be found."
                }
            );
        }

        return NoContent();
    }
}