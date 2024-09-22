using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetApi.Data;
using DotnetApi.Dto;
using DotnetApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContextDapper dataContextDapper;
    private readonly AuthHelpers authHelpers;

    public AuthController(IConfiguration configuration)
    {
        dataContextDapper = new DataContextDapper(configuration);
        authHelpers = new AuthHelpers(configuration);
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLoginDto)
    {
        string hashAndSalt = "SELECT PasswordHash, PasswordSalt FROM TutorialAppSchema.Auth WHERE Email = '" + userForLoginDto.Email + "'";

        UserForLoginConfirmationDto userForLoginConfirmation = dataContextDapper.LoadDataSingle<UserForLoginConfirmationDto>(hashAndSalt);

        byte[] passwordHash = authHelpers.GetPasswordHash(userForLoginDto.Password, userForLoginConfirmation.PasswordSalt);

        /* if (passwordHash == userForLoginConfirmation.PasswordHash)  wont work */

        for (int index = 0; index < passwordHash.Length; index++)
        {
            if (passwordHash[index] != userForLoginConfirmation.PasswordHash[index])
            {
                return StatusCode(401, "incorrect password");
            }
        }

        string userIdSql = "SELECT * FROM TutorialAppSchema.Users WHERE Email = '" + userForLoginDto.Email + "'";

        int userId = dataContextDapper.LoadDataSingle<int>(userIdSql);

        return Ok(new Dictionary<string, string>{
            {"token", authHelpers.createToken(userId)}
        });
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistrationDto)
    {
        if (userForRegistrationDto.Password == userForRegistrationDto.PasswordConfirm)
        {
            string checkUserExist = "SELECT * FROM TutorialAppSchema.Auth WHERE Email = '" + userForRegistrationDto.Email + "'";

            IEnumerable<String> existUser = dataContextDapper.LoadData<string>(checkUserExist);
            if (existUser.Count() == 0)
            {
                byte[] passwordSalt = new byte[128 / 8];
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetNonZeroBytes(passwordSalt);
                }

                byte[] passwordHash = authHelpers.GetPasswordHash(userForRegistrationDto.Password, passwordSalt);

                string addAuth = @"INSERT INTO TutorialAppSchema.Auth (Email, PasswordHash, PasswordSalt) VALUES ('"
                + userForRegistrationDto.Email + "', @PasswordHash, @PasswordSalt)";

                SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                passwordSaltParameter.Value = passwordSalt;

                SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                passwordHashParameter.Value = passwordHash;

                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(passwordSaltParameter);
                sqlParameters.Add(passwordHashParameter);

                if (dataContextDapper.ExecuteSqlWithParameter(addAuth, sqlParameters))
                {
                    string sqlAddUser = @"insert into TutorialAppSchema.Users (FirstName, LastName, Email, Gender, Active) values 
                    ('" + userForRegistrationDto.FirstName
                    + "', '" + userForRegistrationDto.LastName
                    + "' ,'" + userForRegistrationDto.Email
                    + "', '" + userForRegistrationDto.Gender + "', 1)";

                    if (dataContextDapper.ExecuteSql(sqlAddUser))
                    {
                        return Ok();
                    }

                }
                throw new Exception("Failed to register user");

            }
            throw new Exception("User with this email already exist");
        }
        throw new Exception("Passwords do not match");
    }

    [HttpGet("RefreshToken")]
    public IActionResult RefreshToken()
    {
        string userId = User.FindFirst("userId")?.Value + "";

        string userIdSql = @"select UserId from TutorialAppSchema.Users where UserId = " + userId;

        int userIdFromDb = dataContextDapper.LoadDataSingle<int>(userIdSql);

        return Ok(new Dictionary<string, string>{
            {"  ", authHelpers.createToken(userIdFromDb)}
        });
    }
}
