using DotnetApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.WeatherForecastController;

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

    [HttpGet("test/{testValue}")]
    // public IActionResult Test()
    public string[] Test(string testValue)
    {
        string[] responseArray = new[]
        {
            "test1",
            "test2",
            "test3",
            testValue
        };
        return responseArray;
    }
}
