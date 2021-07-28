using SQLite;

namespace GuardAndroidApp.Models
{
    public class User
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int UserTypeId { get; set; }
        public string Token { get; set; }

    }
}