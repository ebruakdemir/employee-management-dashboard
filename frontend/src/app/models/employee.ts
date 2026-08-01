export interface Employee {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    department: string;
    position: string;
    salary: number;
    hireDate: string;
    isActive: boolean;
  }
  
  export interface CreateEmployee {
    firstName: string;
    lastName: string;
    email: string;
    department: string;
    position: string;
    salary: number;
    hireDate: string;
    isActive: boolean;
  }
  
  export interface UpdateEmployee {
    firstName: string;
    lastName: string;
    email: string;
    department: string;
    position: string;
    salary: number;
    hireDate: string;
    isActive: boolean;
  }
  
  export interface PagedResult<T> {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
  }