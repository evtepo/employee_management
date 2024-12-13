namespace EmployeeManagementAPI.DTO;

public class BaseResponse<T> {
    public required PaginationLinks Links { get; set; }
    public required T Data { get; set; }
}

public class PaginationLinks {
    public int? Prev { get; set; }
    public int? Next { get; set; }
    public int? Last { get; set; }
}
