using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Interfaces;
using EmployeeManagement.Api.Mapping;
using EmployeeManagement.Api.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAutoMapper(
    config => { },
    typeof(EmployeeProfile)
);

builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        context.Response.ContentType = "application/json";

        IExceptionHandlerFeature? exceptionFeature =
            context.Features.Get<IExceptionHandlerFeature>();

        await context.Response.WriteAsJsonAsync(new
        {
            statusCode = 500,
            message = "An unexpected error occurred.",
            detail = app.Environment.IsDevelopment()
                ? exceptionFeature?.Error.Message
                : null
        });
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Employee Management API v1"
        );
    });
}

app.UseCors("AngularClient");

app.UseAuthorization();

app.MapControllers();

app.Run();