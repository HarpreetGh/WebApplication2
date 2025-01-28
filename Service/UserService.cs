using Models;

namespace Services {
    public interface IUserInterface
    {
        public Dictionary<int, User> Users { get; set; }
    }

    public class UserService: IUserInterface {
        public Dictionary<int, User> Users { get; set; } = new Dictionary<int, User>();
    }
}