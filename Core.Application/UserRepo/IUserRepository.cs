using Core.Data.Dtos;
using Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.UserRepo
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAllUsers();
        bool IsUserExists(string name);
        bool DeleteUser(User user);
        User GetUserByName(string name);
        bool CreateUser(UserDto user);
        bool UpdateUser(User user);
    }
}
