using SQLite;

namespace GuardAndroidApp.Models
{
    public class Climate
    {
        [PrimaryKey]
        public long Id { get; set; }
        public string Name { get; set; }
    }
}