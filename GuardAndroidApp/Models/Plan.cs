using SQLite;
using System;

namespace GuardAndroidApp.Models
{
    public class Plan
    {
        [PrimaryKey]
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ShiftId { get; set; }
        public DateTime DateTime { get; set; }
        public long LocationId { get; set; }
    }
}