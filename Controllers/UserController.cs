using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    DataContextDapper _dapper;
        string sql=@"INSERT INTO TutorialAppSchema.Computer(
        Motherboard,
        HasWifi,
        HasLTE,
        ReleaseDate,
        Price,
        Videocard
       ) VALUES('"+"ZU234"
       +"','"+true
       +"','"+false
       +"','"+DateTime.Now
       +"','"+234.4M
       +"','"+"QW1920"
       +"')";
    public UserController(IConfiguration config)
    {
       _dapper=new DataContextDapper(config);
    }
    
    [HttpGet("TestConnection1")]
    public DateTime TestConnection1()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }
    [HttpGet("TestConnection2")]
    public IEnumerable<DateTime> TestConnection2()
    {
        return _dapper.LoadData<DateTime>("SELECT GETDATE()");
    }
    [HttpGet("TestConnection3")]
    public bool TestConnection3()
    {
        return _dapper.ExecuteSql(sql);
    }
    [HttpGet("TestConnection4")]
    public int TestConnection4()
    {
        return _dapper.ExecuteSqlrowcount(sql);
    }
    [HttpGet("GetUsers")]
    // public IEnumerable<User> GetUsers()
    public IEnumerable<User> GetUsers()
    {
        string sq=@"SELECT * FROM  TutorialAppSchema.Users";
        IEnumerable<User> users=_dapper.LoadData<User>(sq);
        return users;
        // return new string[] {"user1","user2"};
        // return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        // {
        //     Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //     TemperatureC = Random.Shared.Next(-20, 55),
        //     Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        // })
        // .ToArray();
    }
    [HttpGet("GetSingleUsers/{userId}")]
    public User GetSingleUsers(int userId)
    {
        string sq=@"SELECT * FROM  TutorialAppSchema.Users WHERE UserId="+userId.ToString();
        User user=_dapper.LoadDataSingle<User>(sq);
        return user;
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user){
        string sql=@"
        UPDATE TutorialAppSchema.Users
            SET [FirstName]='"+user.FirstName+
            "',[LastName]='"+user.LastName+
            "',[Email]='"+user.Email+
            "',[Gender]='"+user.Gender+
            "',[Active]='"+user.Active+
            "'WHERE UserId="+user.UserId;
        Console.WriteLine(sql);
        if(_dapper.ExecuteSql(sql)){
            return Ok();
        }
        throw new Exception("Failed to Update User");
    }
    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        string sql=@"INSERT INTO TutorialAppSchema.Users(
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            ) VALUES("+
                "'"+user.FirstName+
                "','"+user.LastName+
                "','"+user.Email+
                "','"+user.Gender+
                "','"+user.Active+
                "')";
            Console.WriteLine(sql);
            if(_dapper.ExecuteSql(sql)){
                return Ok();
            }
            throw new Exception("Failed to add user");
    }
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
      string sql=@"DELETE FROM TutorialAppSchema.Users WHERE UserId="+userId.ToString();
      Console.WriteLine(sql);
            if(_dapper.ExecuteSql(sql)){
                return Ok();
            }
            throw new Exception("Failed to Delete user");  
    }
}

