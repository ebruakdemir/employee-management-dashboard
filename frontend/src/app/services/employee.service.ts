import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpParams
} from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  CreateEmployee,
  Employee,
  PagedResult,
  UpdateEmployee
} from '../models/employee';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private readonly apiUrl =
    'http://localhost:5215/api/Employees';

  constructor(
    private http: HttpClient
  ) {}

  getEmployees(
    search: string = '',
    department: string = '',
    sort: string = '',
    page: number = 1,
    pageSize: number = 10
  ): Observable<PagedResult<Employee>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (search.trim()) {
      params = params.set(
        'search',
        search.trim()
      );
    }

    if (department.trim()) {
      params = params.set(
        'department',
        department.trim()
      );
    }

    if (sort.trim()) {
      params = params.set(
        'sort',
        sort.trim()
      );
    }

    return this.http.get<PagedResult<Employee>>(
      this.apiUrl,
      { params }
    );
  }

  createEmployee(
    employee: CreateEmployee
  ): Observable<Employee> {
    return this.http.post<Employee>(
      this.apiUrl,
      employee
    );
  }

  updateEmployee(
    id: number,
    employee: UpdateEmployee
  ): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/${id}`,
      employee
    );
  }

  deleteEmployee(
    id: number
  ): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}