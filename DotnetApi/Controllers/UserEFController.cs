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
        }));
    }

    [HttpGet("getUsers")]
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _dataContextEF.Users.ToList<User>();
        return users;
    }

    [HttpGet("getSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        User? user = _dataContextEF.Users.Where(x => x.UserId == userId).FirstOrDefault<User>();

        if (user != null)
        {
            return user;
        }
        throw new Exception("Failed to get user");
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

        User? userDb = _dataContextEF.Users.Where(x => x.UserId == userId).FirstOrDefault<User>();

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

}