using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetAPI.Models;

namespace DotnetAPI.Data
{
    public class UserRepository : IUserRepository
    {
        DataContextEF _entityFramework;
        public UserRepository(IConfiguration config)
        {
        _entityFramework=new DataContextEF(config);
        }
        public bool SaveChanges(){
            return _entityFramework.SaveChanges()>0;
        }
        //public bool AddEntity<T>(T entityToAdd)
        public void AddEntity<T>(T entityToAdd)
        {
            if(entityToAdd!=null)
            {
            _entityFramework.Add(entityToAdd);
            // return true;
            }
            // return false;
        }
        public void RemoveEntity<T>(T entityToAdd)
        {
            if(entityToAdd!=null)
            {
            _entityFramework.Remove(entityToAdd);
            // return true;
            }
            // return false;
        }
        public IEnumerable<User> GetUsers()
        {
        IEnumerable<User> users=_entityFramework.Users.ToList<User>();
        return users;
        }
        public User GetSingleUsers(int userId)
        {
        User? user=_entityFramework.Users
        .Where(u=>u.UserId==userId)
        .FirstOrDefault<User>();
        if(user!=null){
            return user;
        }
        throw new Exception("Failed to get User");
        }
    }
}