import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';
import {
  FormsModule,
  NgForm
} from '@angular/forms';

import {
  CreateEmployee,
  Employee
} from './models/employee';

import {
  EmployeeService
} from './services/employee.service';

type SortField =
  | 'name'
  | 'department'
  | 'hireDate'
  | 'salary';

type SortDirection = 'asc' | 'desc';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  employees: Employee[] = [];

  isLoading = false;
  errorMessage = '';

  searchTerm = '';
  selectedDepartment = '';

  sortField: SortField = 'name';
  sortDirection: SortDirection = 'asc';

  currentPage = 1;
  pageSize = 5;
  totalCount = 0;
  totalPages = 0;

  showCreateModal = false;
  isSavingEmployee = false;
  createEmployeeError = '';

  newEmployee: CreateEmployee =
    this.createEmptyEmployee();

  constructor(
    private employeeService: EmployeeService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.employeeService
      .getEmployees(
        '',
        '',
        '',
        this.currentPage,
        this.pageSize
      )
      .subscribe({
        next: response => {
          this.employees = response.items;
          this.totalCount = response.totalCount;
          this.totalPages = response.totalPages;
          this.currentPage = response.page;

          this.isLoading = false;

          this.changeDetectorRef.markForCheck();
        },

        error: error => {
          console.error(
            'Employee loading error:',
            error
          );

          this.errorMessage =
            'Employees could not be loaded.';

          this.isLoading = false;

          this.changeDetectorRef.markForCheck();
        }
      });
  }

  get activeEmployeeCount(): number {
    return this.employees.filter(
      employee => employee.isActive
    ).length;
  }

  get departments(): string[] {
    return [
      ...new Set(
        this.employees
          .map(employee => employee.department)
          .filter(department => Boolean(department))
      )
    ].sort(
      (first, second) =>
        first.localeCompare(second)
    );
  }

  get filteredEmployees(): Employee[] {
    const search =
      this.searchTerm
        .trim()
        .toLowerCase();

    const filtered =
      this.employees.filter(employee => {
        const matchesSearch =
          !search ||
          employee.firstName
            ?.toLowerCase()
            .includes(search) ||
          employee.lastName
            ?.toLowerCase()
            .includes(search) ||
          employee.email
            ?.toLowerCase()
            .includes(search) ||
          employee.department
            ?.toLowerCase()
            .includes(search) ||
          employee.position
            ?.toLowerCase()
            .includes(search);

        const matchesDepartment =
          !this.selectedDepartment ||
          employee.department ===
            this.selectedDepartment;

        return (
          matchesSearch &&
          matchesDepartment
        );
      });

    return [...filtered].sort(
      (
        firstEmployee,
        secondEmployee
      ) =>
        this.compareEmployees(
          firstEmployee,
          secondEmployee
        )
    );
  }

  get pageNumbers(): number[] {
    return Array.from(
      {
        length: this.totalPages
      },
      (_, index) => index + 1
    );
  }

  openCreateModal(): void {
    this.newEmployee =
      this.createEmptyEmployee();

    this.createEmployeeError = '';
    this.showCreateModal = true;
  }

  closeCreateModal(): void {
    if (this.isSavingEmployee) {
      return;
    }

    this.showCreateModal = false;
    this.createEmployeeError = '';
  }

  saveEmployee(form: NgForm): void {
    if (
      form.invalid ||
      this.isSavingEmployee
    ) {
      form.control.markAllAsTouched();
      return;
    }

    this.isSavingEmployee = true;
    this.createEmployeeError = '';

    this.employeeService
      .createEmployee(this.newEmployee)
      .subscribe({
        next: () => {
          this.isSavingEmployee = false;
          this.showCreateModal = false;

          this.newEmployee =
            this.createEmptyEmployee();

          this.currentPage = 1;

          this.loadEmployees();

          this.changeDetectorRef.markForCheck();
        },

        error: error => {
          console.error(
            'Create employee error:',
            error
          );

          this.isSavingEmployee = false;

          if (error.status === 409) {
            this.createEmployeeError =
              'An employee with this email already exists.';
          } else if (error.status === 400) {
            this.createEmployeeError =
              'Please check the form fields.';
          } else {
            this.createEmployeeError =
              'Employee could not be created.';
          }

          this.changeDetectorRef.markForCheck();
        }
      });
  }

  goToPage(page: number): void {
    if (
      page < 1 ||
      page > this.totalPages ||
      page === this.currentPage
    ) {
      return;
    }

    this.currentPage = page;
    this.loadEmployees();
  }

  previousPage(): void {
    this.goToPage(
      this.currentPage - 1
    );
  }

  nextPage(): void {
    this.goToPage(
      this.currentPage + 1
    );
  }

  changePageSize(): void {
    this.currentPage = 1;
    this.loadEmployees();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.selectedDepartment = '';
    this.sortField = 'name';
    this.sortDirection = 'asc';
  }

  getInitials(
    employee: Employee
  ): string {
    const firstInitial =
      employee.firstName
        ?.charAt(0) ?? '';

    const lastInitial =
      employee.lastName
        ?.charAt(0) ?? '';

    return (
      `${firstInitial}${lastInitial}`
        .toUpperCase() || '?'
    );
  }

  private createEmptyEmployee():
    CreateEmployee {
    return {
      firstName: '',
      lastName: '',
      email: '',
      department: '',
      position: '',
      salary: 0,
      hireDate:
        new Date()
          .toISOString()
          .substring(0, 10),
      isActive: true
    };
  }

  private compareEmployees(
    firstEmployee: Employee,
    secondEmployee: Employee
  ): number {
    let comparison = 0;

    switch (this.sortField) {
      case 'name': {
        const firstName =
          `${firstEmployee.firstName} ${firstEmployee.lastName}`;

        const secondName =
          `${secondEmployee.firstName} ${secondEmployee.lastName}`;

        comparison =
          firstName.localeCompare(secondName);

        break;
      }

      case 'department':
        comparison =
          firstEmployee.department.localeCompare(
            secondEmployee.department
          );

        break;

      case 'hireDate':
        comparison =
          new Date(
            firstEmployee.hireDate
          ).getTime() -
          new Date(
            secondEmployee.hireDate
          ).getTime();

        break;

      case 'salary':
        comparison =
          firstEmployee.salary -
          secondEmployee.salary;

        break;
    }

    return this.sortDirection === 'asc'
      ? comparison
      : -comparison;
  }
}