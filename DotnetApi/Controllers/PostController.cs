using DotnetApi.Data;
using DotnetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly DataContextDapper dataContextDapper;

    public PostController(IConfiguration configuration)
    {
        dataContextDapper = new DataContextDapper(configuration);
    }

    [HttpGet("Posts")]
    public IEnumerable<Post> GetPosts()
    {
        string sqlPost = @"SELECT * FROM TutorialAppSchema.Post";

        return dataContextDapper.LoadData<Post>(sqlPost);
    }

    [HttpGet("PostSingle/{postId}")]
    public IEnumerable<Post> GetPostSingle(int postId)
    {
        string sqlPostSingle = @"SELECT * FROM TutorialAppSchema.Post where PostId" + postId.ToString();

        return dataContextDapper.LoadData<Post>(sqlPostSingle);
    }

    [HttpGet("PostByUser/{userId}")]
    public IEnumerable<Post> GetPostByUser(int userId)
    {
        string sqlPostByUser = @"SELECT * FROM TutorialAppSchema.Post where UserId" + userId.ToString();

        return dataContextDapper.LoadData<Post>(sqlPostByUser);
    }

    [HttpGet("MyPost")]
    public IEnumerable<Post> GetMyPost()
    {
        string sqlPostByPost = @"SELECT * FROM TutorialAppSchema.Post where UserId" + this.User.FindFirst("UserId")?.Value;

        return dataContextDapper.LoadData<Post>(sqlPostByPost);
    }

    [HttpGet("Post")]
    public IActionResult AddPost(PostAddDto postAddDto)
    {
        string sql = @"INSERT INTO
        TutorialAppSchema.Post (
        PostId,
        UserId,
        PostContent,
        PostTitle,
        PostCreated,
        PostUpdated
    )
        VALUES(" + this.User.FindFirst("userId")?.Value + ", '"
        + postAddDto.PostTitle + ", '"
        + postAddDto.PostContent + ", '"
        + "', GETDATE(), GETDATE() )";

        if (dataContextDapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to create new post ");
    }

    [HttpPut("Post")]
    public IActionResult EditPost(PostEditDto postEditDto)
    {
        string sql = @"UPDATE TutorialAppSchema.Post
        SET
            PostContent = '" + postEditDto.PostContent +
            "', PostTitle =  '" + postEditDto.PostTitle +
            @"', PostUpdated = GETDATE()  
        WHERE
            PostId = " + postEditDto.PostId.ToString() + "and UserId = "
            + this.User.FindFirst("userId")?.Value;

        if (dataContextDapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to edit post ");
    }

    [HttpDelete("Post/{postId}")]
    public IActionResult DeletePost(int postId)
    {
        string sql = @"DELETE FROM TutorialAppSchema.Post WHERE PostId = " + postId.ToString() + "and UserId = "
            + this.User.FindFirst("userId")?.Value;

        if (dataContextDapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to edit post");
    }
}