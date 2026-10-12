using AdvancedLinqAsyncDemo.Models;

namespace AdvancedLinqAsyncDemo.Services;

public class EmployeeReportService
{
    public async Task<List<Employee>> GetEmployeesAsync(
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(1000, cancellationToken);

        return new List<Employee>
        {
            new() { Id = 1, Name = "Monika", DepartmentId = 1,
                Salary = 65000, IsActive = true },

            new() { Id = 2, Name = "Rahul", DepartmentId = 2,
                Salary = 45000, IsActive = true },

            new() { Id = 3, Name = "Priya", DepartmentId = 1,
                Salary = 75000, IsActive = true },

            new() { Id = 4, Name = "Amit", DepartmentId = 3,
                Salary = 55000, IsActive = false },

            new() { Id = 5, Name = "Sneha", DepartmentId = 1,
                Salary = 50000, IsActive = true },

            new() { Id = 6, Name = "Karan", DepartmentId = 2,
                Salary = 40000, IsActive = false }
        };
    }

    public async Task<List<Department>> GetDepartmentsAsync(
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(1500, cancellationToken);

        return new List<Department>
        {
            new() { Id = 1, Name = "IT" },
            new() { Id = 2, Name = "HR" },
            new() { Id = 3, Name = "Finance" }
        };
    }

    public async Task<List<Customer>> GetCustomersAsync(
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(1200, cancellationToken);

        return new List<Customer>
        {
            new()
            {
                Id = 1,
                Name = "Customer A",
                Orders = new List<Order>
                {
                    new() { Id = 101, CustomerId = 1, Amount = 1500 },
                    new() { Id = 102, CustomerId = 1, Amount = 2500 }
                }
            },
            new()
            {
                Id = 2,
                Name = "Customer B",
                Orders = new List<Order>
                {
                    new() { Id = 103, CustomerId = 2, Amount = 4000 },
                    new() { Id = 104, CustomerId = 2, Amount = 1000 }
                }
            },
            new()
            {
                Id = 3,
                Name = "Customer C",
                Orders = new List<Order>
                {
                    new() { Id = 105, CustomerId = 3, Amount = 3000 },
                    new() { Id = 106, CustomerId = 3, Amount = 2000 }
                }
            }
        };
    }

    public async Task<List<EmployeeReportDto>> GetEmployeeReportAsync(
        CancellationToken cancellationToken = default)
    {
        // Start independent operations before awaiting them.
        Task<List<Employee>> employeesTask =
            GetEmployeesAsync(cancellationToken);

        Task<List<Department>> departmentsTask =
            GetDepartmentsAsync(cancellationToken);

        await Task.WhenAll(employeesTask, departmentsTask);

        List<Employee> employees = await employeesTask;
        List<Department> departments = await departmentsTask;

        var report = employees
            .Where(e => e.IsActive)
            .Join(
                departments,
                employee => employee.DepartmentId,
                department => department.Id,
                (employee, department) => new EmployeeReportDto
                {
                    EmployeeName = employee.Name,
                    DepartmentName = department.Name,
                    Salary = employee.Salary
                })
            .OrderByDescending(e => e.Salary)
            .ToList();

        return report;
    }
}