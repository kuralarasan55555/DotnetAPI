using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;
[ApiController]
[Route("[controller]")]
public class UserJobInfoEFController : ControllerBase
{
   DataContextEF _entityframework;
   IUserRepository _userRepository;

   public UserJobInfoEFController(IConfiguration config,IUserRepository userRepository)
   {
    _entityframework=new DataContextEF(config);
    _userRepository=userRepository;
   }
   [HttpGet("GetUsersJob")]
   public IEnumerable<UserJobInfo> GetUsersJob()
   {
    IEnumerable<UserJobInfo> usersjob=_entityframework.UserJobInfo.ToList<UserJobInfo>();
    return usersjob;
   }
   [HttpGet("GetSingheUsersJobInfo/{userId}")]
   public UserJobInfo GetSingheUsersJobInfo(int userId)
   {
     UserJobInfo? userjob=_entityframework.UserJobInfo
        .Where(u=>u.UserId==userId)
        .FirstOrDefault<UserJobInfo>();
        if(userjob!=null){
            return userjob;
        }
        throw new Exception("Failed to get User");
   }
   [HttpPut("EditUserJob")]
   public IActionResult EditUserJob(UserJobInfo userjobinfo)
   {
      UserJobInfo? userDb=_entityframework.UserJobInfo
        .Where(u=>u.UserId==userjobinfo.UserId)
        .FirstOrDefault<UserJobInfo>();
        Console.WriteLine(userDb);
        if(userDb!=null)
        {
        userDb.UserId=userjobinfo.UserId;
        userDb.Department=userjobinfo.Department;
        userDb.JobTitle=userjobinfo.JobTitle;
        if(_userRepository.SaveChanges()){
            return Ok();
        }
        throw new Exception("Failed to Update User");
        }
        throw new Exception("Failed to Get User");
   }
   [HttpPost("Adduserjob")]
   public IActionResult Adduserjob(UserJobInfo user){
        UserJobInfo? userDb=new UserJobInfo();
         userDb.UserId=user.UserId;
         userDb.JobTitle=user.JobTitle;
         userDb.Department=user.Department;
         _userRepository.AddEntity<UserJobInfo>(userDb);
        if(_userRepository.SaveChanges()){
            return Ok();
        }
        throw new Exception("Failed to Add User");
   }
   [HttpDelete("Deleteuserjob/{userId}")]
   public IActionResult Deleteuserjob(int userId)
   {
      UserJobInfo? userDb=_entityframework.UserJobInfo
        .Where(u=>u.UserId==userId)
        .FirstOrDefault<UserJobInfo>();
        if(userDb!=null){
        _userRepository.RemoveEntity<UserJobInfo>(userDb);
        if(_userRepository.SaveChanges()){
            return Ok();
        }
        throw new Exception("Failed to Delete User");
    }
    throw new Exception("Failed to Get User");
   }
}