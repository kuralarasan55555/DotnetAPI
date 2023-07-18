
using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController:ControllerBase
    {
         private readonly DataContextDapper _dapper;
        public PostController(IConfiguration config)
        {
          _dapper=new DataContextDapper(config);
        }
        
        [HttpGet("Posts/{postId}/{UserId}/{searchparam}")]
        public IEnumerable<Post> GetPosts(int postId=0,int UserId=0,string searchparam="None")
        {
            string sql=@"EXEC TutorialAppSchema.spPosts_Get";
            string parameter="";
            DynamicParameters sqlParameter=new DynamicParameters();
            if(postId!=0)
            {
               parameter+=", @PostId=@PostIdParam";
               sqlParameter.Add("@PostIdParam",postId,DbType.Int32);
            }
            if(UserId!=0)
            {
                parameter+=", @UserId=@UserIdParam";
                sqlParameter.Add("@UserIdParam",UserId,DbType.Int32);
            }
            string search=searchparam.ToLower();
            if(search!= "none")
            {
                parameter+=", @SearchValue=@SearchValueParam";
                sqlParameter.Add("@SearchValueParam",searchparam,DbType.String);
            }
            if(parameter.Length>0)
            {
            sql+=parameter.Substring(1);
            }
            Console.WriteLine(sql);
            return _dapper.LoadDataWithParameters<Post>(sql,sqlParameter);
        }
        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql=@"EXEC TutorialAppSchema.spPosts_Get @UserId=@useridparam";
            DynamicParameters sqlParameter=new DynamicParameters();
            sqlParameter.Add("@useridparam",this.User.FindFirst("userID")?.Value,DbType.Int32);
            return _dapper.LoadDataWithParameters<Post>(sql,sqlParameter);
        }
        [HttpPut("PostUpsert")]
        public IActionResult PostUpsert(Post postToupsert)
        {
            string sql=@"EXEC TutorialAppSchema.spPosts_Upsert
                    @UserId=@useridparam
                    @PostTitle=@posttitleparam
                    ,@PostContent=@postcontentparam";

           DynamicParameters sqlParameter=new DynamicParameters();
           sqlParameter.Add("@useridparam",this.User.FindFirst("userID")?.Value,DbType.Int32);

           sqlParameter.Add("@posttitleparam",postToupsert.PostTitle,DbType.String);

           sqlParameter.Add("@postcontentparam",postToupsert.PostContent,DbType.String);

            if(postToupsert.PostId>0){
                sql+=", @PostId=@postidparam";
                sqlParameter.Add("@postidparam",postToupsert.PostId,DbType.Int32);
            }
            Console.WriteLine(sql);
            if(_dapper.ExecuteSqlWithParameters(sql,sqlParameter))
            {
                return Ok();
            }
            throw new Exception("Failed to Upsert Post");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql=@"EXEC TutorialAppSchema.spPost_Delete @PostId=postidparam,@UserId=useridparam";
            DynamicParameters sqlParameter=new DynamicParameters();
            sqlParameter.Add("@useridparam",this.User.FindFirst("userID")?.Value,DbType.Int32);
            sqlParameter.Add("@postidparam",postId,DbType.Int32);
            Console.WriteLine(sql);
            if(_dapper.ExecuteSqlWithParameters(sql,sqlParameter)){
                return Ok();
            }
            throw new Exception("Failed to delete post");
        }
    }
}