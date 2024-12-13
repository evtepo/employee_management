namespace EmployeeManagementAPI.DTO;

public class CreateDepartment {
    public string Title { get; set; } = string.Empty;
}

public class UpdateDepartment : CreateDepartment {}
