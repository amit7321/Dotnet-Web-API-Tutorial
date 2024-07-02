namespace DotnetApi.Data;

public class UserRepository
{
    DataContextEF dataContextEF;
    public UserRepository(IConfiguration configuration)
    {
        dataContextEF = new DataContextEF(configuration);
    }

    public bool SaveChanges()
    {
        return dataContextEF.SaveChanges() > 0 ;
    }

    public void AddEntity<T>(T entity) 
    { 
        if (entity != null)
        {
            dataContextEF.Add(entity);
        }

    }


}