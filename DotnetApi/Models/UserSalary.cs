namespace DotnetApi.Models;

public partial class UserSalary
{
    public int UserId { get; set; }
    public string Salary { get; set; } = "";

    public UserSalary()
    {
    }
}