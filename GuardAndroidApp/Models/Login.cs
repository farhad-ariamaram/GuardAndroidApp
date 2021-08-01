using SQLite;

namespace GuardAndroidApp.Models
{
    public class Login
    {
        [PrimaryKey]
        public long Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
    }
}