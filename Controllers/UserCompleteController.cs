using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;
// [Authorize]
[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly ReusableSql _reusableSql;

    public UserCompleteController(IConfiguration config)
    {
       _dapper=new DataContextDapper(config);
       _reusableSql=new ReusableSql(config);
    }
    
    [HttpGet("GetSingleUsers/{userId}/{Active}")]
    public IEnumerable<UserComplete> GetSingleUsers(int userId,bool Active)
    {
        string sq=@"EXEC TutorialAppSchema.spUsers_Get";
        string stringParameter=""; 
        DynamicParameters sqlParameters=new DynamicParameters();
        if(userId!=0){
            stringParameter+=", @UserId=@UserIdParameter";
            sqlParameters.Add("@UserIdParameter",userId,DbType.Int32);
        }
        if(Active){
            stringParameter+=", @Active=@ActiveParameter";
            sqlParameters.Add("@ActiveParameter",userId,DbType.Boolean);
        }
        if(stringParameter.Length>0)
        {
            sq+=stringParameter.Substring(1);
            sqlParameters.Add("@UserIdParameter",userId,DbType.Int32);
        }
        Console.WriteLine(sq);
        IEnumerable<UserComplete> user=_dapper.LoadDataWithParameters<UserComplete>(sq,sqlParameters);
        return user;
    }

    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserComplete user){
        // string sql=@"EXEC TutorialAppSchema.spUser_Upsert
        //      @FirstName=@FirstNameParameter
        //     ,@LastName=@LastNameParameter
        //     ,@Email=@EmailParameter
        //     ,@Gender=@GenderParameter
        //     ,@Active=@ActiveParameter
        //     ,@JobTitle=@JobTitleParameter
        //     ,@Department=@DepartmentParameter
        //     ,@Salary=@SalaryParameter
        //     ,@UserId=@UserIdParameter";

        // DynamicParameters sqlParameters=new DynamicParameters();

        // sqlParameters.Add("@UserIdParameter",user.UserId,DbType.Int32);
        // sqlParameters.Add("@LastNameParameter",user.LastName,DbType.String);
        // sqlParameters.Add("@FirstNameParameter",user.FirstName,DbType.String);
        // sqlParameters.Add("@EmailParameter",user.Email,DbType.String);
        // sqlParameters.Add("@GenderParameter",user.Gender,DbType.String);
        // sqlParameters.Add("@ActiveParameter",user.Active,DbType.Boolean);
        // sqlParameters.Add("@JobTitleParameter",user.JobTitle,DbType.String);
        // sqlParameters.Add("@DepartmentParameter",user.Department,DbType.String);
        // sqlParameters.Add("@SalaryParameter",user.Salary,DbType.Decimal);

        // Console.WriteLine(sql);

        if(_reusableSql.UpsertUser(user)){
            return Ok();
        }
        throw new Exception("Failed to Update User");
    }
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
      string sql=@"EXEC TutorialAppSchema.spUser_Delete 
      @UserId=@UserIdParameter";
      DynamicParameters sqlParameters=new DynamicParameters();
      sqlParameters.Add("@UserIdParameter",userId,DbType.Int32);
      Console.WriteLine(sql);
            if(_dapper.ExecuteSqlWithParameters(sql,sqlParameters)){
                return Ok();
            }
            throw new Exception("Failed to Delete user");  
    }
}

