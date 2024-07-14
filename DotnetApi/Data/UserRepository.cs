using DotnetApi.Models;

namespace DotnetApi.Data;

public class UserRepository : IUserRepository
{
    DataContextEF dataContextEF;
    public UserRepository(IConfiguration configuration)
    {
        dataContextEF = new DataContextEF(configuration);
    }

    public bool SaveChanges()
    {
        return dataContextEF.SaveChanges() > 0;
    }

    public void AddEntity<T>(T entity)
    {
        if (entity != null)
        {
            dataContextEF.Add(entity);
        }
    }

    public void RemoveEntity<T>(T entity)
    {
        if (entity != null)
        {
            dataContextEF.Remove(entity);
        }
    }

    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = dataContextEF.Users.ToList<User>();
        return users;
    }

    public User GetSingleUser(int userId)
    {
        User? user = dataContextEF.Users.Where(x => x.UserId == userId).FirstOrDefault<User>();

        if (user != null)
        {
            return user;
        }
        throw new Exception("Failed to get user");
    }

    public UserSalary GetSingleUserSalary(int userId)
    {
        UserSalary? userSalary = dataContextEF.UserSalary.Where(x => x.UserId == userId).FirstOrDefault<UserSalary>();

        if (userSalary != null)
        {
            return userSalary;
        }
        throw new Exception("Failed to get user salary");
    }

    public UserJobInfo GetSingleUserjobInfo(int userId)
    {
        UserJobInfo? userJobInfo = dataContextEF.UserJobInfo.Where(x => x.UserId == userId).FirstOrDefault<UserJobInfo>();

        if (userJobInfo != null)
        {
            return userJobInfo;
        }
        throw new Exception("Failed to get user job info");
    }

}