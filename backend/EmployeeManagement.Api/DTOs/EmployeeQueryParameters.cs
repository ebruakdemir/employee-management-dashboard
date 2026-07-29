namespace EmployeeManagement.Api.DTOs;

public class EmployeeQueryParameters
{
    public string? Search { get; set; }

    public string? Department { get; set; }

    public string? Sort { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}