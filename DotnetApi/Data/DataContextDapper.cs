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
        IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        return dbConnection.Query<T>(sql);
    }

    public T LoadDataSingle<T>(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        return dbConnection.QuerySingle<T>(sql);
    }

    public bool ExecuteSql(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        return dbConnection.Execute(sql) > 0;
    }

    public int ExecuteSqlWithCountRows(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        return dbConnection.Execute(sql);
    }

    public bool ExecuteSqlWithParameter(string sql, List<SqlParameter> parameters)
    {
        SqlCommand sqlCommand = new SqlCommand(sql);

        foreach (SqlParameter parameter in parameters)
        {
            sqlCommand.Parameters.Add(parameter);
        }

        SqlConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        dbConnection.Open();

        sqlCommand.Connection = dbConnection;
        int rowsAffected = sqlCommand.ExecuteNonQuery();

        dbConnection.Close();

        return rowsAffected > 0;
    }

    internal bool ExecuteSqlWithParameter(string sqlAddUser, DynamicParameters sqlParametersUser)
    {
        throw new NotImplementedException();
    }
}