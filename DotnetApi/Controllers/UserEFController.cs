using AutoMapper;
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
    IUserRepository iuserRepository;
    IMapper mapper;

    public UserEFController(IConfiguration configuration, IUserRepository userRepository)
    {
        _dataContextEF = new DataContextEF(configuration);

        iuserRepository = userRepository;

        mapper = new Mapper(new MapperConfiguration(mp =>
        {
            mp.CreateMap<UserDto, User>();
            mp.CreateMap<UserSalary, UserSalary>();
            mp.CreateMap<UserJobInfo, UserJobInfo>();

        }));
    }

    [HttpGet("getUsers")]
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = iuserRepository.GetUsers();
        return users;
    }

    [HttpGet("getSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        return iuserRepository.GetSingleUser(userId);
    }

    [HttpPut("editUser")]
    public IActionResult EditUser(User user)
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

        if (iuserRepository.SaveChanges())
        {
            return Ok();
        }

        throw new Exception("Failed to update User");

    }

    [HttpPost]
    public IActionResult AddUser(UserDto user)
    {
        User userDb = mapper.Map<User>(user);

        iuserRepository.AddEntity<User>(userDb);

        if (iuserRepository.SaveChanges())
        {
            return Ok();
        }

        throw new Exception("Failed to add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {

        User? userDb = iuserRepository.GetSingleUser(userId);

        if (userDb != null)
        {
            iuserRepository.RemoveEntity<User>(userDb);

            if (iuserRepository.SaveChanges())
            {
                return Ok();
            }

            throw new Exception("Failed to delete User");
        }

        throw new Exception("Failed to get user");

    }

    [HttpGet("UserSalary/{userId}")]
    public UserSalary GetUserSalary(int userId)
    {
        return iuserRepository.GetSingleUserSalary(userId);
    }

    [HttpPost("UserSalary")]
    public IActionResult PostUserSalary(UserSalary userSalary)
    {
        iuserRepository.AddEntity<UserSalary>(userSalary);

        if (iuserRepository.SaveChanges())
        {
            return Ok();
        }
        throw new Exception("User salary save");
    }

    [HttpPut("UserSalary")]
    public IActionResult PutUserSalary(UserSalary userSalary)
    {
        UserSalary? userSalaryDb = iuserRepository.GetSingleUserSalary(userSalary.UserId);

        if (userSalaryDb != null)
        {
            mapper.Map(userSalaryDb, userSalaryDb);

            if (iuserRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to add User");

        }
        throw new Exception("Failed to find user salary");

    }

    [HttpDelete("UserSalary/{userId}")]
    public IActionResult DeleteUserSalary(UserSalary userSalary)
    {
        UserSalary? userDelete = iuserRepository.GetSingleUserSalary(userSalary.UserId);

        if (userDelete != null)
        {
            iuserRepository.RemoveEntity<UserSalary>(userDelete);

            if (iuserRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to Delete User");

        }
        throw new Exception("Failed to find user salary");

    }

}