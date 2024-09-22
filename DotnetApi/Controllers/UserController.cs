using DotnetApi.Data;
using DotnetApi.Dto;
using DotnetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers  ;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    DataContextDapper _dataContextDapper;

    public UserController(IConfiguration configuration)
    {
        _dataContextDapper = new DataContextDapper(configuration);
    }

    [HttpGet("testConnection")]
    public DateTime TestConnection()
    {
        return _dataContextDapper.LoadDataSingle<DateTime>("select getdate()");
    }

    [HttpGet("getUsers")]
    public IEnumerable<User> getUsers()
    {
        string sql = @"
            select 
            userid,
            firstname,
            lastname,
            email,
            gender,
            active
        from TutorialAppSchema.Users";
        IEnumerable<User> users = _dataContextDapper.LoadData<User>(sql);
        return users;
    }

    [HttpGet("getSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        string sql = @"
            select 
            userid,
            firstname,
            lastname,
            email,
            gender,
            active
        from TutorialAppSchema.Users where UserId = " + userId.ToString();
        User user = _dataContextDapper.LoadDataSingle<User>(sql);
        return user;
    }

    [HttpPut("editUser")]
    public IActionResult editUser(User user)
    {
        string sql = @"update TutorialAppSchema.Users
            set 
                firstname = ' " + user.FirstName +
                     "', lastname = '" + user.LastName +
                     "', email = '" + user.Email +
                     "', gender = '" + user.Gender +
                     "', active = '" + user.Active +
                     "where UserId = " + user.UserId;
        
        if(_dataContextDapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to update User");
        
    }

    [HttpPost]
    public IActionResult addUser(UserDto user)
    {
                string sql = @"insert into TutorialAppSchema.Users
            (FirstName, LastName, Email, Gender, Active) values 
                (firstname = ' " + user.FirstName +
                     "', '" + user.LastName +
                     "', '" + user.Email +
                     "', '" + user.Gender +
                     "', '" + user.Active + ")";
                    
        
        if(_dataContextDapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to update User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult deleteUser(int userId)
    {
        string sql = @"delete from TutorialAppSchema.Users where UserId =" + userId.ToString();

        if(_dataContextDapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to delete User");
    }       
    
}