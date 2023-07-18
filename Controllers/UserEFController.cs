using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    IUserRepository _userRepository;
    IMapper _mapper;
    public UserEFController(IConfiguration config,IUserRepository userRepository)
    {
       _userRepository=userRepository;
       _mapper=new Mapper(new MapperConfiguration(cfg=>{
        cfg.CreateMap<UserToAddDto,User>();
       }));
    }
    [HttpGet("GetUsers")]
    // public IEnumerable<User> GetUsers()
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users=_userRepository.GetUsers();
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
        return _userRepository.GetSingleUsers(userId);
    }
    [HttpPut("EditUser")]
    public IActionResult EditUser(User user){
        User? userDb=_userRepository.GetSingleUsers(user.UserId);
        // User? userDb=_mapper.Map<User>(user);
        if(userDb!=null){
         userDb.Active=user.Active;
         userDb.FirstName=user.FirstName;
         userDb.LastName=user.LastName;
         userDb.Email=user.Email;
         userDb.Gender=user.Gender;
        if(_userRepository.SaveChanges()){
            return Ok();
        }
        throw new Exception("Failed to Update User");
        }
        throw new Exception("Failed to Get User");
    }
    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
         User? userDb=new User();
         userDb.Active=user.Active;
         userDb.FirstName=user.FirstName;
         userDb.LastName=user.LastName;
         userDb.Email=user.Email;
         userDb.Gender=user.Gender;
         _userRepository.AddEntity<User>(userDb);
        if(_userRepository.SaveChanges()){
            return Ok();
        }
        throw new Exception("Failed to Add User");
    }
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
       User? userDb=_userRepository.GetSingleUsers(userId);
        if(userDb!=null){
        _userRepository.RemoveEntity<User>(userDb);
        if(_userRepository.SaveChanges()){
            return Ok();
        }
        throw new Exception("Failed to Delete User");
    }
    throw new Exception("Failed to Get User");
}
}

