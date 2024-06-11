using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotnetApi.Data;

public class DataContextDapper
{
    private readonly IConfiguration _configuration;

    public DataContextDapper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IEnumerable<T> LoadData<T>(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings"));
        return dbConnection.Query<T>(sql);
    }
    
    public T LoadDataSingle<T>(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings"));
        return dbConnection.QuerySingle<T>(sql);
    }

    public bool ExecuteSql(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings"));
        return dbConnection.Execute(sql) > 0;
    }
    
    public int ExecuteSqlWithCountRows(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings"));
        return dbConnection.Execute(sql);
    }
}