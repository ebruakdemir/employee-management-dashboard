namespace EmployeeManagement.Api.DTOs;

public class EmployeeUpdateResult
{
    public bool IsSuccessful { get; set; }

    public bool EmployeeNotFound { get; set; }

    public bool EmailAlreadyExists { get; set; }
}