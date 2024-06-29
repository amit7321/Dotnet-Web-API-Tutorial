using DotnetApi.Data;
using DotnetApi.Dto;
using DotnetApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    DataContextEF _dataContextEF;

    public UserEFController(IConfiguration configuration)
    {
        _dataContextEF = new DataContextEF(configuration);
    }

    [HttpGet("getUsers")]
    public IEnumerable<User> getUsers()
    {
        IEnumerable<User> users = _dataContextEF.Users.ToList<User>();
        return users;
    }

    [HttpGet("getSingleUser/{userId}")]
    public User getSingleUser(int userId)
    {
        User? user = _dataContextEF.Users.Where(x => x.UserId == userId).FirstOrDefault<User>();

        if (user != null)
        {
            return user;
        }
        throw new Exception("Failed to get user");
    }

    [HttpPut("editUser")]
    public IActionResult editUser(User user)
    {
        User? userDb = _dataContextEF.Users.Where(x => x.UserId == user.UserId).FirstOrDefault<User>();

        if (userDb != null)
        {
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Gender = user.Gender;
            userDb.Email = user.Email;
            userDb.Active = user.Active;
        }

        if (_dataContextEF.SaveChanges() > 0)
        {
            return Ok();
        }

        throw new Exception("Failed to update User");

    }

    [HttpPost]
    public IActionResult addUser(UserDto user)
    {
        User userDb = new User();

        userDb.FirstName = user.FirstName;
        userDb.LastName = user.LastName;
        userDb.Gender = user.Gender;
        userDb.Email = user.Email;
        userDb.Active = user.Active;

        _dataContextEF.Add(userDb);

        if (_dataContextEF.SaveChanges() > 0)
        {
            return Ok();
        }

        throw new Exception("Failed to add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {

        User? userDb = _dataContextEF.Users.Where(x => x.UserId == userId).FirstOrDefault<User>();

        if (userDb != null)
        {
            _dataContextEF.Users.Remove(userDb);

            if (_dataContextEF.SaveChanges() > 0)
            {
                return Ok();
            }

        }

        throw new Exception("Failed to delete User");

    }

}