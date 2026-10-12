using System.Diagnostics;
using AdvancedLinqAsyncDemo.Models;
using AdvancedLinqAsyncDemo.Services;

EmployeeReportService service = new();

Console.WriteLine("==================================");
Console.WriteLine(" ADVANCED LINQ + ASYNC DEMO");
Console.WriteLine("==================================");

// ==================================================
// 1. RETRIEVE DATA
// ==================================================

List<Employee> employees = await service.GetEmployeesAsync();
List<Department> departments = await service.GetDepartmentsAsync();
List<Customer> customers = await service.GetCustomersAsync();


// ==================================================
// 2. GROUP BY DEPARTMENT
// ==================================================

Console.WriteLine("\n===== GROUP BY DEPARTMENT =====");

var employeeGroups = employees.GroupBy(e => e.DepartmentId);

foreach (var group in employeeGroups)
{
    Console.WriteLine($"Department ID: {group.Key}");

    foreach (Employee employee in group)
    {
        Console.WriteLine($"  {employee.Name}");
    }
}


// ==================================================
// 3. GROUP BY + COUNT
// ==================================================

Console.WriteLine("\n===== EMPLOYEE COUNT BY DEPARTMENT =====");

var employeeCounts = employees
    .GroupBy(e => e.DepartmentId)
    .Select(group => new
    {
        DepartmentId = group.Key,
        EmployeeCount = group.Count()
    });

foreach (var item in employeeCounts)
{
    Console.WriteLine(
        $"Department ID: {item.DepartmentId}, Count: {item.EmployeeCount}");
}


// ==================================================
// 4. GROUP BY + AVERAGE
// ==================================================

Console.WriteLine("\n===== AVERAGE SALARY BY DEPARTMENT =====");

var averageSalaries = employees
    .GroupBy(e => e.DepartmentId)
    .Select(group => new
    {
        DepartmentId = group.Key,
        AverageSalary = group.Average(e => e.Salary)
    });

foreach (var item in averageSalaries)
{
    Console.WriteLine(
        $"Department ID: {item.DepartmentId}, Average Salary: ₹{item.AverageSalary:N2}");
}


// ==================================================
// 5. SUM, MIN, MAX
// ==================================================

Console.WriteLine("\n===== SALARY STATISTICS =====");

decimal totalSalary = employees.Sum(e => e.Salary);
decimal minimumSalary = employees.Min(e => e.Salary);
decimal maximumSalary = employees.Max(e => e.Salary);

Console.WriteLine($"Total Salary: ₹{totalSalary:N2}");
Console.WriteLine($"Minimum Salary: ₹{minimumSalary:N2}");
Console.WriteLine($"Maximum Salary: ₹{maximumSalary:N2}");


// ==================================================
// 6. JOIN EMPLOYEES + DEPARTMENTS
// ==================================================

Console.WriteLine("\n===== EMPLOYEE DEPARTMENT JOIN =====");

var employeeDetails = employees.Join(
    departments,
    employee => employee.DepartmentId,
    department => department.Id,
    (employee, department) => new
    {
        EmployeeName = employee.Name,
        DepartmentName = department.Name,
        employee.Salary
    });

foreach (var item in employeeDetails)
{
    Console.WriteLine(
        $"{item.EmployeeName} | {item.DepartmentName} | ₹{item.Salary:N2}");
}


// ==================================================
// 7. SELECTMANY - FLATTEN ALL ORDERS
// ==================================================

Console.WriteLine("\n===== ALL CUSTOMER ORDERS =====");

var allOrders = customers.SelectMany(customer => customer.Orders);

foreach (Order order in allOrders)
{
    Console.WriteLine(
        $"Order ID: {order.Id}, Customer ID: {order.CustomerId}, Amount: ₹{order.Amount:N2}");
}


// ==================================================
// 8. SELECTMANY + SUM
// ==================================================

Console.WriteLine("\n===== TOTAL ORDER VALUE =====");

decimal totalOrderValue = customers
    .SelectMany(customer => customer.Orders)
    .Sum(order => order.Amount);

Console.WriteLine($"Total Order Value: ₹{totalOrderValue:N2}");


// ==================================================
// 9. PROJECTION INTO DTO
// ==================================================

Console.WriteLine("\n===== EMPLOYEE DTO PROJECTION =====");

List<EmployeeReportDto> employeeDtos = employees
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
    .ToList();

foreach (EmployeeReportDto dto in employeeDtos)
{
    Console.WriteLine(
        $"{dto.EmployeeName} | {dto.DepartmentName} | ₹{dto.Salary:N2}");
}


// ==================================================
// 10. COMBINED REPORT
// ==================================================

Console.WriteLine("\n===== ACTIVE EMPLOYEE REPORT =====");

List<EmployeeReportDto> activeReport = employees
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

foreach (EmployeeReportDto item in activeReport)
{
    Console.WriteLine(
        $"{item.EmployeeName} | {item.DepartmentName} | ₹{item.Salary:N2}");
}


// ==================================================
// 11. SEQUENTIAL ASYNC EXECUTION
// ==================================================

Console.WriteLine("\n===== SEQUENTIAL ASYNC EXECUTION =====");

Stopwatch sequentialTimer = Stopwatch.StartNew();

List<Employee> sequentialEmployees =
    await service.GetEmployeesAsync();

List<Department> sequentialDepartments =
    await service.GetDepartmentsAsync();

List<Customer> sequentialCustomers =
    await service.GetCustomersAsync();

sequentialTimer.Stop();

Console.WriteLine(
    $"Employees retrieved: {sequentialEmployees.Count}");

Console.WriteLine(
    $"Departments retrieved: {sequentialDepartments.Count}");

Console.WriteLine(
    $"Customers retrieved: {sequentialCustomers.Count}");

Console.WriteLine(
    $"Sequential elapsed time: {sequentialTimer.ElapsedMilliseconds} ms");


// ==================================================
// 12. PARALLEL ASYNC EXECUTION WITH TASK.WHENALL
// ==================================================

Console.WriteLine("\n===== TASK.WHENALL EXECUTION =====");

Stopwatch parallelTimer = Stopwatch.StartNew();

Task<List<Employee>> employeesTask =
    service.GetEmployeesAsync();

Task<List<Department>> departmentsTask =
    service.GetDepartmentsAsync();

Task<List<Customer>> customersTask =
    service.GetCustomersAsync();

await Task.WhenAll(
    employeesTask,
    departmentsTask,
    customersTask);

List<Employee> parallelEmployees = await employeesTask;
List<Department> parallelDepartments = await departmentsTask;
List<Customer> parallelCustomers = await customersTask;

parallelTimer.Stop();

Console.WriteLine(
    $"Employees retrieved: {parallelEmployees.Count}");

Console.WriteLine(
    $"Departments retrieved: {parallelDepartments.Count}");

Console.WriteLine(
    $"Customers retrieved: {parallelCustomers.Count}");

Console.WriteLine(
    $"Task.WhenAll elapsed time: {parallelTimer.ElapsedMilliseconds} ms");


// ==================================================
// 13. ASYNC EMPLOYEE REPORT SERVICE
// ==================================================

Console.WriteLine("\n===== ASYNC EMPLOYEE REPORT =====");

List<EmployeeReportDto> asyncReport =
    await service.GetEmployeeReportAsync();

foreach (EmployeeReportDto item in asyncReport)
{
    Console.WriteLine(
        $"{item.EmployeeName} | {item.DepartmentName} | ₹{item.Salary:N2}");
}


// ==================================================
// 14. CANCELLATION TOKEN
// ==================================================

Console.WriteLine("\n===== CANCELLATION DEMO =====");

using CancellationTokenSource cancellationSource =
    new(TimeSpan.FromMilliseconds(300));

try
{
    Console.WriteLine("Starting employee retrieval...");

    await service.GetEmployeesAsync(cancellationSource.Token);

    Console.WriteLine("Retrieval completed.");
}
catch (OperationCanceledException)
{
    Console.WriteLine("Employee retrieval was cancelled.");
}

Console.WriteLine("\nDemo completed.");