namespace EmployeeManagementAPI.DTO;

public class EmployeeCreate
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Department { get; set; } = string.Empty;
}

public class EmployeeUpdate : EmployeeCreate {}
