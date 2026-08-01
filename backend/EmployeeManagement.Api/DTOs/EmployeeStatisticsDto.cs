namespace EmployeeManagement.Api.DTOs;

public class EmployeeStatisticsDto
{
    public int TotalEmployees { get; set; }

    public int ActiveEmployees { get; set; }

    public int DepartmentCount { get; set; }

    public decimal AverageSalary { get; set; }
}