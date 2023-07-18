using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;
[ApiController]
[Route("[controller]")]
public class UserJobInfoController : ControllerBase
{
   DataContextDapper _dapper;
   public UserJobInfoController(IConfiguration config)
   {
    _dapper=new DataContextDapper(config);
   }
   [HttpGet("GetUsersJob")]
   public IEnumerable<UserJobInfo> GetUsersJob()
   {
    string sql=@"SELECT * FROM TutorialAppSchema.UserJobInfo";
    IEnumerable<UserJobInfo> usersjob=_dapper.LoadData<UserJobInfo>(sql);
    return usersjob;
   }
   [HttpGet("GetSingheUsersJobInfo/{userId}")]
   public UserJobInfo GetSingheUsersJobInfo(int userId)
   {
     string sql=@"SELECT * FROM  TutorialAppSchema.UserJobInfo WHERE UserId="+userId.ToString();
     return _dapper.LoadDataSingle<UserJobInfo>(sql);
   }
   [HttpPut("EditUserJob")]
   public IActionResult EditUserJob(UserJobInfo userjobinfo)
   {
     string sql=@"
        UPDATE TutorialAppSchema.UserJobInfo
            SET [JobTitle]='"+userjobinfo.JobTitle+
            "',[Department]='"+userjobinfo.Department+
            "' WHERE UserId="+userjobinfo.UserId;
        Console.WriteLine(sql);
        if(_dapper.ExecuteSql(sql)){
            return Ok();
        }
        throw new Exception("Failed to Update UserJob");
   }
   [HttpPost("Adduserjob")]
   public IActionResult Adduserjob(UserJobInfo userjob){
            string sql=@"INSERT INTO TutorialAppSchema.UserJobInfo(
                [UserId],
                [JobTitle],
                [Department]
            )VALUES("+userjob.UserId+
                "','"+userjob.JobTitle+
                "','"+userjob.Department+
                "')";
            Console.WriteLine(sql);
            if(_dapper.ExecuteSql(sql)){
                return Ok();
            }
            throw new Exception("Failed to add userjob");
   }
   [HttpDelete("Deleteuserjob/{userId}")]
   public IActionResult Deleteuserjob(int userId)
   {
      string sql=@"DELETE FROM TutorialAppSchema.UserJobInfo WHERE UserId="+userId.ToString();
      Console.WriteLine(sql);
            if(_dapper.ExecuteSql(sql)){
                return Ok();
            }
            throw new Exception("Failed to Delete userjob");
   }
}