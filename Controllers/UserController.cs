using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace Controllers {
    [ApiController]
    [Route("/users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController: ControllerBase {
        private IUserInterface userService;

        public UserController(IUserInterface US) {
            userService = US;
        }
        
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUser()
        {
            return Ok(userService.Users.Values);
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            if (userService.Users.TryGetValue(id, out var user))
            {
                return Ok(user);
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult<User> CreateUser(User user)
        {
            if (userService.Users.ContainsKey(user.Id))
            {
                return Conflict("User with this ID already exists.");
            }
            userService.Users[user.Id] = user;
            return Created($"/users/{user.Id}", user);
        }

        [HttpPut("{id}")]
        public ActionResult<User> UpdateUser(int id, User updatedUser)
        {
            if (userService.Users.ContainsKey(id))
            {
                userService.Users[id] = updatedUser;
                return Ok(updatedUser);
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id) =>
            userService.Users.Remove(id) ? NoContent() : NotFound();
    }
}