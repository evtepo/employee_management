using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using EmployeeManagementAPI.Data;
using EmployeeManagementAPI.DTO;
using EmployeeManagementAPI.Models;

namespace EmployeeManagementAPI.Controllers;

[ApiController]
[Route("api/v1/employee/")]
public class EmployeeController : ControllerBase {
    private readonly AppDbContext _context;

    public EmployeeController(AppDbContext context) {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<List<Employee>>>> GetEmployees(
        [FromQuery]int page = 1,
        [FromQuery]int size = 10,
        [FromQuery] string? firstName = null,
        [FromQuery] string? lastName = null,
        [FromQuery] DateOnly? dateOfBirth = null,
        [FromQuery] string? departmentTitle = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool descending = false

    ) {
        if (size > 100) size = 100;

        IQueryable<Employee> query = _context.Employees.Include(e => e.Department);

        if (!string.IsNullOrEmpty(firstName)) {
            query = query.Where(e => e.FirstName.Contains(firstName));
        }
        if (!string.IsNullOrEmpty(lastName)) {
            query = query.Where(e => e.LastName.Contains(lastName));
        }
        if (dateOfBirth.HasValue) {
            query = query.Where(e => e.DateOfBirth == dateOfBirth.Value);
        }
        if (!string.IsNullOrEmpty(departmentTitle)) {
            query = query.Where(e => e.Department.Title.Contains(departmentTitle));
        }

        if (!string.IsNullOrEmpty(sortBy)) {
            if (sortBy.Equals("FirstName", StringComparison.OrdinalIgnoreCase)) {
                query = descending ? query.OrderByDescending(e => e.FirstName) : query.OrderBy(e => e.FirstName);
            } else if (sortBy.Equals("LastName", StringComparison.OrdinalIgnoreCase)) {
                query = descending ? query.OrderByDescending(e => e.LastName) : query.OrderBy(e => e.LastName);
            } else if (sortBy.Equals("DateOfBirth", StringComparison.OrdinalIgnoreCase)) {
                query = descending ? query.OrderByDescending(e => e.DateOfBirth) : query.OrderBy(e => e.DateOfBirth);
            } else if (sortBy.Equals("DepartmentTitle", StringComparison.OrdinalIgnoreCase)) {
                query = descending ? query.OrderByDescending(e => e.Department.Title) : query.OrderBy(e => e.Department.Title);
            }
        }

        var offset = (page - 1) * size;
        var employees = await query.Skip(offset).Take(size).ToListAsync();

        int totalRows = await _context.Employees.CountAsync();
        int? prevPage = page > 1 ? page - 1 : null;
        int? nextPage = employees.Count() == size ? page + 1 : null;
        int lastPage =  (int)Math.Ceiling((double)totalRows / size);

        var result = new BaseResponse<List<Employee>> {
            Links = new PaginationLinks {
                Prev = prevPage,
                Next = nextPage,
                Last = lastPage
            },
            Data = employees,
        };

        return result;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> SingleEmployee(int id) {
        var employee = await _context.Employees.Include(employee => employee.Department).FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null) {
            return NotFound();
        }

        return employee;
    }

    [HttpPost]
    public async Task<ActionResult<Employee>> CreateEmployee([FromBody]EmployeeCreate employee) {
        var department = await CheckDepartment(employee.Department);

        var newEmployee = new Employee
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            DateOfBirth = employee.DateOfBirth,
            Department = department,
        };

        await _context.Employees.AddAsync(newEmployee);
        await _context.SaveChangesAsync();

        return StatusCode(201, newEmployee);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Employee>> UpdateEmployee(
        int id,
        [FromBody]EmployeeUpdate employee
    ) {
        var existingEmployee = await _context.Employees
        .Include(e => e.Department)
        .FirstOrDefaultAsync(e => e.Id == id);
        if (existingEmployee is null)
        {
            return NotFound($"Employee with ID {id} not found.");
        }

        var department = await CheckDepartment(employee.Department);

        existingEmployee.FirstName = employee.FirstName;
        existingEmployee.LastName = employee.LastName;
        existingEmployee.DateOfBirth = employee.DateOfBirth;
        existingEmployee.Department = department;

        await _context.SaveChangesAsync();

        return Ok(existingEmployee);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<string>> DeleteEmployee(int id) {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (employee is null) {
            return NotFound($"Can't delete employee with ID {id}.");
        }

        _context.Employees.Remove(employee);

        await _context.SaveChangesAsync();

        return Ok("Successfully deleted.");
    }

    private async Task<Department> CheckDepartment(string departmentName) {
        var department = await _context.Departments.FirstOrDefaultAsync(d => d.Title == departmentName);
        if (department is null) {
            department = new Department {Title = departmentName};
            await _context.Departments.AddAsync(department);

            await _context.SaveChangesAsync();
        }

        return department;
    }
};
