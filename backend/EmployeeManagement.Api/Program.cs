using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Mapping;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(EmployeeProfile));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenApi();

var app = builder.Build();

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

// Şimdilik kapalı:
 // app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();