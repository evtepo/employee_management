namespace EmployeeManagementAPI.Models;

public class Department
{
    public int Id { get; set; }
    public required string Title { get; set; } = string.Empty;
}
