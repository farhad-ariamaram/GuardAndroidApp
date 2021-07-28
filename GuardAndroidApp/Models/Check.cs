using SQLite;

namespace GuardAndroidApp.Models
{
    public class Check
    {
        [PrimaryKey]
        public long Id { get; set; }
        public string Name { get; set; }
    }
}