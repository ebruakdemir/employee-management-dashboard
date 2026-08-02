# Emplora — Employee Management Dashboard

Emplora is a full-stack employee management dashboard built with Angular, ASP.NET Core Web API, Entity Framework Core, and SQL Server.

The application allows users to manage employee records, view company-wide statistics, search and filter employees, and perform complete CRUD operations through a responsive dashboard.

## Features

- View employees with server-side pagination
- Create new employee records
- Edit existing employees
- Delete employees with a confirmation dialog
- Search by name, email, department, or position
- Filter employees by department
- Sort employees by name, department, hire date, or salary
- View company-wide employee statistics
- Display active employee percentage
- View department and average salary summaries
- Light and dark themes
- Responsive pastel dashboard design
- Toast notifications
- Frontend and backend validation
- API error handling

## Technologies

### Frontend

- Angular
- TypeScript
- HTML
- CSS
- Angular Forms
- Angular HttpClient

### Backend

- ASP.NET Core Web API
- C#
- Entity Framework Core
- SQL Server
- Dependency Injection
- DTO pattern
- Service layer architecture

### Development Tools

- Visual Studio Code
- Docker
- Git
- GitHub
- Swagger / OpenAPI

## Project Structure

```text
employee-management-dashboard
├── backend
│   └── EmployeeManagement.Api
│       ├── Controllers
│       ├── Data
│       ├── DTOs
│       ├── Interfaces
│       ├── Models
│       ├── Services
│       └── Program.cs
│
├── frontend
│   └── src
│       └── app
│           ├── models
│           ├── services
│           ├── app.ts
│           ├── app.html
│           └── app.css
│
└── README.md
```

## Architecture

```text
Angular Frontend
        |
        | HTTP requests
        v
ASP.NET Core Controller
        |
        v
Employee Service
        |
        v
Entity Framework Core
        |
        v
SQL Server
```

The controller handles HTTP requests, while the service layer contains employee-related business logic.

DTOs are used to control the data transferred between the frontend and backend.

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Employees` | Returns a paginated employee list |
| GET | `/api/Employees/{id}` | Returns a single employee |
| GET | `/api/Employees/statistics` | Returns company-wide statistics |
| POST | `/api/Employees` | Creates a new employee |
| PUT | `/api/Employees/{id}` | Updates an employee |
| DELETE | `/api/Employees/{id}` | Deletes an employee |

## Statistics Response

Example response from:

```text
GET /api/Employees/statistics
```

```json
{
  "totalEmployees": 8,
  "activeEmployees": 6,
  "departmentCount": 4,
  "averageSalary": 45250
}
```

## Getting Started

### Requirements

- .NET SDK
- Node.js
- Angular CLI
- Docker Desktop
- Git

## Database Setup

Start the SQL Server Docker container:

```bash
docker start employee-sql
```

Check that it is running:

```bash
docker ps
```

## Backend Setup

```bash
cd backend/EmployeeManagement.Api
dotnet restore
dotnet ef database update
dotnet run
```

The API normally runs at:

```text
http://localhost:5215
```

The OpenAPI document is available at:

```text
http://localhost:5215/openapi/v1.json
```

## Frontend Setup

```bash
cd frontend
npm install
ng serve
```

Open the application at:

```text
http://localhost:4200
```

## Validation

The application validates employee data on both the frontend and backend.

Examples:

- First name is required
- Last name is required
- Email must be valid
- Department is required
- Position is required
- Salary must be zero or greater
- Hire date is required
- Duplicate email addresses are prevented

## Error Handling

The frontend handles common API errors:

- `400 Bad Request` for invalid form data
- `404 Not Found` when an employee does not exist
- `409 Conflict` when an email is already registered
- General server and connection errors

Errors are displayed through form messages and toast notifications.

## Responsive Design

The interface adapts to:

- Desktop screens
- Tablets
- Mobile devices

On smaller screens, the sidebar becomes a horizontal navigation menu and employee cards move into a single-column layout.

## What I Learned

Through this project, I practised:

- Building REST APIs with ASP.NET Core
- Creating Angular standalone components
- Connecting Angular to an external API
- Using Entity Framework Core with SQL Server
- Designing DTOs and service layers
- Implementing server-side pagination
- Creating complete CRUD operations
- Managing Angular forms and validation
- Handling API errors
- Building responsive and interactive dashboards
- Using Git with structured commit messages

## Future Improvements

- Authentication and authorization
- User roles
- Department management endpoints
- Employee profile pictures
- Charts and advanced analytics
- Unit and integration tests
- CSV or PDF export
- Cloud deployment

## Author

Ebru Akdemir