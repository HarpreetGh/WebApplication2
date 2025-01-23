using Microsoft.AspNetCore.Mvc;

public class User {
    public int Id {get; set;}
    public string Name {get; set;}
}

[ApiController]
[Route("[controller]")]
public class UserController: Controller {
    [HttpGet("GetUser")]
    public ActionResult<User> GetUser(int id) {
        return new User { Id = id, Name = $"User {id}" };
    }

}