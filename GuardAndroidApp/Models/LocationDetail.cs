using SQLite;
using System;

namespace GuardAndroidApp.Models
{
    public class LocationDetail
    {
        [PrimaryKey]
        public long Id { get; set; }
        public long LocationId { get; set; }
        public long? ClimateId { get; set; }
        public long CheckId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public bool Check { get; set; }
    }
}