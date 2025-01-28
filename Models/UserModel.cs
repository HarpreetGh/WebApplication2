namespace Models {
    public class User
    {
        public int Id { get; set; }
        required public string Name { get; set; }
        required public string Email { get; set; }
        public int Age { get; set; }
    }
}