import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';

import { EmployeeService } from './services/employee.service';
import { Employee } from './models/employee';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  employees: Employee[] = [];
  errorMessage = '';
  isLoading = false;

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

    this.employeeService.getEmployees().subscribe({
      next: response => {
        console.log('API response:', response);

        this.employees = response.items;
        this.isLoading = false;

        this.changeDetectorRef.markForCheck();
      },

      error: error => {
        console.error('API error:', error);

        this.errorMessage =
          'Employees could not be loaded.';

        this.isLoading = false;

        this.changeDetectorRef.markForCheck();
      }
    });
  }
}