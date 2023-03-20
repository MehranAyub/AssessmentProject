using Core.Application;
using Core.Data;
using Core.Data.Dtos;
using Core.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Text.Json.Nodes;

namespace UserInformation.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepositoryWrapper repository;
        private readonly IDatabase _database;
        private string KeyName = "Users";

        public UserController(IRepositoryWrapper repositorywrapper)
        {
            repository = repositorywrapper;
            var connection = ConnectionMultiplexer.Connect("localhost:6379");
            _database = connection.GetDatabase();
        }
        [HttpGet(Name = "GetAllUsers")]
        public PayloadCustom<User> GetAllUsers()
        {
            var users=_database.StringGet(KeyName); //First of all will chaeck if Cache has data then will return persisted data otherwise will check database.
            if (users.IsNullOrEmpty)
            {
                var data = repository.User.GetAllUsers();
                if (data.Count() > 0)
                {
                    string SerializeData = JsonConvert.SerializeObject(data);
                    _database.StringSet(KeyName, SerializeData); //Data persistance at Cache Memory
                    return new PayloadCustom<User>() { EntityList = (List<User>)data, Status = 200 };
                }
                return new PayloadCustom<User>() { Status = 404, ErrorMessage = "No user Exists" };
            }
            _database.KeyDelete(KeyName);
          List<User> userList= JsonConvert.DeserializeObject<List<User>>(users);
            return new PayloadCustom<User>() {EntityList= userList, Status = 200, Message="Persisted Data" };


        }


        [HttpPost]
        public PayloadCustom<User> CreateUser([FromBody] UserDto user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(user.Name.IsNullOrEmpty() || user.Address.IsNullOrEmpty() )
                    {
                        return new PayloadCustom<User>() { Entity =null, Status =406,ErrorMessage="Please fill All fields"  };
                    }
                    bool result=repository.User.IsUserExists(user.Name);
                    if (result)
                    {
                        return new PayloadCustom<User>() {  Status = 409, ErrorMessage = "User with this name Already exists" };

                    }

                    result = repository.User.CreateUser(user);
                    if (result == true)
                    {
                        repository.Save();
                   
                        return new PayloadCustom<User>() { Entity = new User { Address=user.Address,Name=user.Name}, Status = 201, Message="User Created Successfully" };

                    }
                    return new PayloadCustom<User>() { Entity = null, Status = 500 , ErrorMessage = "Internal Server Error" };

                }

                return new PayloadCustom<User>() { Entity = null, Status = 501, ErrorMessage = "Not Implemented" };

            }
            catch
            {
                return new PayloadCustom<User>() { Entity = null, Status = 500, ErrorMessage = "Internal Server Error" };

            }

        }

        [HttpPut]
        public PayloadCustom<User> UpdateUser([FromBody] UserDto userDto)
        {
            try
            {
               

                if (!ModelState.IsValid)
                {
                    return new PayloadCustom<User>() { Status = 400, ErrorMessage = "Bad Request" };
                }

                var user = repository.User.GetUserByName(userDto.Name);
                if (user == null)
                {
                    return new PayloadCustom<User>() { Status = 404, Message = "This user Not Exists" };
                }
                user.Name = userDto.Name;
                user.Address = userDto.Address;

                var result = repository.User.UpdateUser(user);
                if(result != true)
                {
                    return new PayloadCustom<User>() {  Status = 500, ErrorMessage = "Internal Server Error" };

                }
                repository.Save();

                return new PayloadCustom<User>() { Entity = user, Status = 200, Message = "User Updated Successfully" };
            }
            catch (Exception ex)
            {
                return new PayloadCustom<User>() { Status = 500, ErrorMessage = "Internal Server Error"+ex };

            }
        }

        [HttpDelete("{name}")]
        public PayloadCustom<User> DeleteUser(string name)
        {
            try
            {
                var user = repository.User.GetUserByName(name);


                if (user == null)
                {
                    return new PayloadCustom<User>() { Status = 404, Message = "User with this name not Found" };
                }

                var result=repository.User.DeleteUser(user);
                if (result == false)
                {
                    return new PayloadCustom<User>() { Status = 500 , ErrorMessage = "Internal Server Error" };

                }
                repository.Save();

                return new PayloadCustom<User>() { Status = 200,Message = "User Deleted Successfully" };
            }
            catch (Exception ex)
            {
                return new PayloadCustom<User>() { Status = 500, ErrorMessage = "Internal Server Error "+ex };
            }
        }


    }
}
