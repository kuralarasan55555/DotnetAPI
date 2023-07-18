using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers
{
    public class AuthHelper
    {
        private readonly IConfiguration _config;
         private readonly DataContextDapper _dapper;
        public AuthHelper(IConfiguration config)
        {
             _config=config;
             _dapper=new DataContextDapper(config);
        }
        public byte[] GetPasswordHash(string password,byte[] passwordSalt) 
        {
             string passwordSaltPlusString=_config.GetSection("AppSettings:PasswordKey").Value+
                Convert.ToBase64String(passwordSalt);
                    //PRF-PSEUDO RANDOM FUNCTIONALITY
            return KeyDerivation.Pbkdf2(
                password:password,
                salt:Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf:KeyDerivationPrf.HMACSHA256,
                iterationCount:1000000,
                numBytesRequested:256/8);
        }
        public   string CreateToken(int userId)
        {
           Claim[] claims=new Claim[]{
            new Claim("userId",userId.ToString())
           };
           string? tokenKeyString=_config.GetSection("Appsettings:TokenKey").Value;
           SymmetricSecurityKey tokenKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                   (tokenKeyString!=null?tokenKeyString:"")
                );
            SigningCredentials credentials=new SigningCredentials(
                tokenKey,
                SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor descriptor=new SecurityTokenDescriptor()
                {
                    Subject=new ClaimsIdentity(claims),
                    SigningCredentials=credentials,
                    Expires=DateTime.Now.AddDays(1)

                };
                //Token Handler
                JwtSecurityTokenHandler tokenHandler=new JwtSecurityTokenHandler();
                SecurityToken token=tokenHandler.CreateToken(descriptor);//Actual Token
                return tokenHandler.WriteToken(token);
        }
        public bool SetPassword(UserForLoginDto userloginDto){
                   byte[] passwordSalt=new byte[128/8];
                    using (RandomNumberGenerator rn=RandomNumberGenerator.Create())
                    {
                        rn.GetNonZeroBytes(passwordSalt);
                    }
                    byte[] passwordHash=GetPasswordHash(userloginDto.Password,passwordSalt);
                    string sqlAddAuth=@"EXEC TutorialAppSchema.spRegistration_Upsert
                    @Email=@EmailParam,
                    @PasswordHash=@PasswordHashParam,
                    @PasswordSalt=@PasswordSaltParam";
                    // List<SqlParameter> sqlParameters=new List<SqlParameter>();

                    // SqlParameter emailParameter=new SqlParameter("@EmailParam",SqlDbType.VarChar);
                    // emailParameter.Value=userloginDto.Email;
                    // sqlParameters.Add(emailParameter);

                    // SqlParameter passwordSaltParameter=new SqlParameter("@PasswordSaltParam",SqlDbType.VarBinary);
                    // passwordSaltParameter.Value=passwordSalt;
                    // sqlParameters.Add(passwordSaltParameter);

                    // SqlParameter passwordHashParameter=new SqlParameter("@PasswordHashParam",SqlDbType.VarBinary);
                    // passwordHashParameter.Value=passwordHash;
                    // sqlParameters.Add(passwordHashParameter);
                    DynamicParameters sqlParameters=new DynamicParameters();
                    sqlParameters.Add("@EmailParam",userloginDto.Email,DbType.String);
                    sqlParameters.Add("@PasswordHashParam",passwordHash,DbType.Binary);
                    sqlParameters.Add("@PasswordSaltParam",passwordSalt,DbType.Binary);

                    return(_dapper.ExecuteSqlWithParameters(sqlAddAuth,sqlParameters));
        }
    }
}