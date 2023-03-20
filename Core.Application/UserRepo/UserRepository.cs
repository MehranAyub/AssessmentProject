using Core.Data.BaseRepository;
using Core.Data.Entities;
using Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Data.Dtos;

namespace Core.Application.UserRepo
{
    internal class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(RepositoryContext repositoryContext)
             : base(repositoryContext)
        {
        }

        public IEnumerable<User> GetAllUsers()
        {
            return GetAll().ToList();
        }

        public bool IsUserExists(string name)
        {
            bool result = RepositoryContext.User.Any(x => x.Name == name);
            return result;
        }

        public User GetUserByName(string name)
        {
            var result=RepositoryContext.User.FirstOrDefault(x => x.Name == name);
            return result;
        }

        public bool DeleteUser(User user)
        {
            try
            {
                Delete(user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CreateUser(UserDto userDto)
        {
            try
            {
                User user = new()
                {
                    Name = userDto.Name,
                    Address = userDto.Address
                };
                Create(user);
                return true;
            }
            catch
            {
                return false;
            }
               
            
        }
        public bool UpdateUser(User user)
        {
            try
            {
                Update(user);
                return true;
            }
            catch
            {
                return false;
            }


        }
    }
}
