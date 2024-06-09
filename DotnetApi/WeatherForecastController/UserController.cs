using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.WeatherForecastController;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    public UserController()
    {
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
