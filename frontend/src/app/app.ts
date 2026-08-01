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
  Employee,
  UpdateEmployee
} from './models/employee';

import { EmployeeService } from './services/employee.service';

type SortField =
  | 'name'
  | 'department'
  | 'hireDate'
  | 'salary';

type SortDirection = 'asc' | 'desc';

type ToastType = 'success' | 'error';

type DashboardSection =
  | 'dashboard'
  | 'employees'
  | 'departments'
  | 'reports';

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
  pageSize = 6;
  totalCount = 0;
  totalPages = 0;

  activeSection: DashboardSection = 'dashboard';
  isDarkMode = false;

  showEmployeeModal = false;
  isSavingEmployee = false;
  employeeFormError = '';

  editingEmployeeId: number | null = null;

  employeeFormData: CreateEmployee =
    this.createEmptyEmployee();

  showDeleteModal = false;
  employeeToDelete: Employee | null = null;
  isDeletingEmployee = false;

  toastMessage = '';
  toastType: ToastType = 'success';

  private toastTimer?: ReturnType<typeof setTimeout>;

  constructor(
    private employeeService: EmployeeService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadTheme();
    this.loadEmployees();
  }

  get isEditMode(): boolean {
    return this.editingEmployeeId !== null;
  }

  get activeEmployeeCount(): number {
    return this.employees.filter(
      employee => employee.isActive
    ).length;
  }

  get activePercentage(): number {
    if (this.employees.length === 0) {
      return 0;
    }

    return Math.round(
      (
        this.activeEmployeeCount /
        this.employees.length
      ) * 100
    );
  }

  get departments(): string[] {
    return [
      ...new Set(
        this.employees
          .map(employee => employee.department)
          .filter(department => Boolean(department))
      )
    ].sort((first, second) =>
      first.localeCompare(second)
    );
  }

  get departmentCount(): number {
    return this.departments.length;
  }

  get averageSalary(): number {
    if (this.employees.length === 0) {
      return 0;
    }

    const totalSalary =
      this.employees.reduce(
        (total, employee) =>
          total + employee.salary,
        0
      );

    return totalSalary / this.employees.length;
  }

  get filteredEmployees(): Employee[] {
    const search =
      this.searchTerm.trim().toLowerCase();

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
      { length: this.totalPages },
      (_, index) => index + 1
    );
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

  setActiveSection(
    section: DashboardSection
  ): void {
    this.activeSection = section;

    const target =
      document.getElementById(section);

    target?.scrollIntoView({
      behavior: 'smooth',
      block: 'start'
    });
  }

  toggleTheme(): void {
    this.isDarkMode = !this.isDarkMode;

    localStorage.setItem(
      'employee-dashboard-theme',
      this.isDarkMode ? 'dark' : 'light'
    );
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.selectedDepartment = '';
    this.sortField = 'name';
    this.sortDirection = 'asc';
  }

  openCreateModal(): void {
    this.editingEmployeeId = null;

    this.employeeFormData =
      this.createEmptyEmployee();

    this.employeeFormError = '';
    this.showEmployeeModal = true;

    this.changeDetectorRef.markForCheck();
  }

  openEditModal(employee: Employee): void {
    this.editingEmployeeId = employee.id;

    this.employeeFormData = {
      firstName: employee.firstName,
      lastName: employee.lastName,
      email: employee.email,
      department: employee.department,
      position: employee.position,
      salary: employee.salary,
      hireDate: employee.hireDate.substring(0, 10),
      isActive: employee.isActive
    };

    this.employeeFormError = '';
    this.showEmployeeModal = true;

    this.changeDetectorRef.markForCheck();
  }

  closeEmployeeModal(): void {
    if (this.isSavingEmployee) {
      return;
    }

    this.showEmployeeModal = false;
    this.employeeFormError = '';
    this.editingEmployeeId = null;

    this.changeDetectorRef.markForCheck();
  }

  saveEmployee(form: NgForm): void {
    if (
      form.invalid ||
      this.isSavingEmployee
    ) {
      form.control.markAllAsTouched();
      return;
    }

    if (this.isEditMode) {
      this.updateEmployee();
      return;
    }

    this.createEmployee(form);
  }

  openDeleteModal(employee: Employee): void {
    this.employeeToDelete = employee;
    this.showDeleteModal = true;

    this.changeDetectorRef.markForCheck();
  }

  closeDeleteModal(): void {
    if (this.isDeletingEmployee) {
      return;
    }

    this.employeeToDelete = null;
    this.showDeleteModal = false;

    this.changeDetectorRef.markForCheck();
  }

  confirmDeleteEmployee(): void {
    if (
      !this.employeeToDelete ||
      this.isDeletingEmployee
    ) {
      return;
    }

    const employeeId =
      this.employeeToDelete.id;

    this.isDeletingEmployee = true;

    this.employeeService
      .deleteEmployee(employeeId)
      .subscribe({
        next: () => {
          this.isDeletingEmployee = false;
          this.showDeleteModal = false;
          this.employeeToDelete = null;

          if (
            this.employees.length === 1 &&
            this.currentPage > 1
          ) {
            this.currentPage--;
          }

          this.loadEmployees();

          this.showToast(
            'Employee deleted successfully.'
          );
        },

        error: error => {
          console.error(
            'Delete employee error:',
            error
          );

          this.isDeletingEmployee = false;

          this.showToast(
            error.status === 404
              ? 'Employee could not be found.'
              : 'Employee could not be deleted.',
            'error'
          );

          this.changeDetectorRef.markForCheck();
        }
      });
  }

  showToast(
    message: string,
    type: ToastType = 'success'
  ): void {
    this.toastMessage = message;
    this.toastType = type;

    if (this.toastTimer) {
      clearTimeout(this.toastTimer);
    }

    this.toastTimer = setTimeout(() => {
      this.toastMessage = '';
      this.changeDetectorRef.markForCheck();
    }, 3000);

    this.changeDetectorRef.markForCheck();
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

  getInitials(employee: Employee): string {
    const firstInitial =
      employee.firstName?.charAt(0) ?? '';

    const lastInitial =
      employee.lastName?.charAt(0) ?? '';

    return (
      `${firstInitial}${lastInitial}`
        .toUpperCase() || '?'
    );
  }

  getAvatarClass(employee: Employee): string {
    const classes = [
      'avatar-lavender',
      'avatar-mint',
      'avatar-peach',
      'avatar-pink',
      'avatar-blue'
    ];

    return classes[
      Math.abs(employee.id) % classes.length
    ];
  }

  private createEmployee(form: NgForm): void {
    this.isSavingEmployee = true;
    this.employeeFormError = '';

    this.employeeService
      .createEmployee(this.employeeFormData)
      .subscribe({
        next: () => {
          this.isSavingEmployee = false;
          this.showEmployeeModal = false;

          const emptyEmployee =
            this.createEmptyEmployee();

          this.employeeFormData = emptyEmployee;
          form.resetForm(emptyEmployee);

          this.currentPage = 1;
          this.loadEmployees();

          this.showToast(
            'Employee created successfully.'
          );
        },

        error: error => {
          this.handleSaveError(
            error,
            'Employee could not be created.'
          );
        }
      });
  }

  private updateEmployee(): void {
    if (this.editingEmployeeId === null) {
      return;
    }

    this.isSavingEmployee = true;
    this.employeeFormError = '';

    const updateData: UpdateEmployee = {
      ...this.employeeFormData
    };

    this.employeeService
      .updateEmployee(
        this.editingEmployeeId,
        updateData
      )
      .subscribe({
        next: () => {
          this.isSavingEmployee = false;
          this.showEmployeeModal = false;
          this.editingEmployeeId = null;

          this.loadEmployees();

          this.showToast(
            'Employee updated successfully.'
          );
        },

        error: error => {
          this.handleSaveError(
            error,
            'Employee could not be updated.'
          );
        }
      });
  }

  private handleSaveError(
    error: any,
    defaultMessage: string
  ): void {
    console.error(
      'Save employee error:',
      error
    );

    this.isSavingEmployee = false;

    if (error.status === 409) {
      this.employeeFormError =
        'An employee with this email already exists.';
    } else if (error.status === 400) {
      this.employeeFormError =
        'Please check the form fields.';
    } else if (error.status === 404) {
      this.employeeFormError =
        'Employee could not be found.';
    } else {
      this.employeeFormError =
        defaultMessage;
    }

    this.showToast(
      this.employeeFormError,
      'error'
    );

    this.changeDetectorRef.markForCheck();
  }

  private loadTheme(): void {
    const savedTheme =
      localStorage.getItem(
        'employee-dashboard-theme'
      );

    this.isDarkMode =
      savedTheme === 'dark';
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