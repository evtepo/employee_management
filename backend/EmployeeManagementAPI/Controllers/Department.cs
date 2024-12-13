using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using EmployeeManagementAPI.Data;
using EmployeeManagementAPI.DTO;
using EmployeeManagementAPI.Models;

namespace EmployeeManagementAPI.Controllers;

[ApiController]
[Route("api/v1/department/")]
public class DepartmentController : ControllerBase {
    private readonly AppDbContext _context;

    public  DepartmentController(AppDbContext context) {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<List<Department>>>> GetDepartments(
        [FromQuery]int page = 1,
        [FromQuery]int size = 10
    ) {
        if (size > 100) {
            size = 100;
        }

        var preQuery = _context.Departments;

        var offset = (page - 1) * size;
        var department = await preQuery.Skip(offset).Take(size).ToListAsync();

        int totalRows = await _context.Departments.CountAsync();
        int? prevPage = page > 1 ? page - 1 : null;
        int? nextPage = department.Count() == size ? page + 1 : null;
        int lastPage =  (int)Math.Ceiling((double)totalRows / size);

        var result = new BaseResponse<List<Department>> {
            Links = new PaginationLinks {
                Prev = prevPage,
                Next = nextPage,
                Last = lastPage
            },
            Data = department,
        };

        return result;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Department>> SingleDepartment(int id) {
        var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
        if (department is null) {
            return NotFound();
        }

        return department;
    }

    [HttpPost]
    public async Task<ActionResult<Department>> CreateDepartment([FromBody]CreateDepartment department) {
        var newDepartment = new Department{
            Title = department.Title
        };

        await _context.Departments.AddAsync(newDepartment);
        await _context.SaveChangesAsync();

        return StatusCode(201, newDepartment);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Department>> ChangeDepartment(
        int id,
        [FromBody]UpdateDepartment department
    ) {
        var existingDepartment = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
        if (existingDepartment is null) {
            return NotFound($"Department with ID {id} not found.");
        }

        existingDepartment.Title = department.Title;

        await _context.SaveChangesAsync();

        return Ok(existingDepartment);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<string>> DeleteDepartment(int id) {
        var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
        if (department is null) {
            return NotFound($"Can't delete department with ID {id}.");
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return Ok("Successfully deleted.");
    }
}