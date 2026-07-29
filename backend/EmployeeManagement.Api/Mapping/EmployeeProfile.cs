using AutoMapper;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Models;

namespace EmployeeManagement.Api.Mapping;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<CreateEmployeeDto, Employee>();

        CreateMap<UpdateEmployeeDto, Employee>();

        CreateMap<Employee, EmployeeResponseDto>();
    }
}