using System.Data;
using System.Security.Cryptography;
using System.Text;
using DotnetApi.Data;
using DotnetApi.Dto;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetApi.Controllers;

public class AuthController : ControllerBase
{
    private readonly DataContextDapper dataContextDapper;
    private readonly IConfiguration configuration;

    public AuthController(IConfiguration config)
    {
        dataContextDapper = new DataContextDapper(config);
        configuration = config;
    }

    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLoginDto)
    {
        return Ok();
    }

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

                string passwordSaltString = configuration.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

                byte[] passwordHash = KeyDerivation.Pbkdf2(
                    password: userForRegistrationDto.Password,
                    salt: Encoding.ASCII.GetBytes(passwordSaltString),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 1000000,
                    numBytesRequested: 256 / 8
                );

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
                    return Ok();

                }
                

            }
            throw new Exception("User with this email already exist");
        }
        throw new Exception("Passwords do not match");
    }
}