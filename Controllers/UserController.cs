using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Controllers {
    [ApiController]
    [Route("/users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController: ControllerBase {
        private Dictionary<int, User> users = new Dictionary<int, User>();

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Ok(users.Values);
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            if (users.TryGetValue(id, out var user))
            {
                return Ok(user);
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult<User> CreateUser(User user)
        {
            if (users.ContainsKey(user.Id))
            {
                return Conflict("User with this ID already exists.");
            }
            users[user.Id] = user;
            return Created($"/users/{user.Id}", user);
        }

        [HttpPut("{id}")]
        public ActionResult<User> UpdateUser(int id, User updatedUser)
        {
            if (users.ContainsKey(id))
            {
                users[id] = updatedUser;
                return Ok(updatedUser);
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id) =>
            users.Remove(id) ? NoContent() : NotFound();
    }
}