using SQLite;
using System;

namespace GuardAndroidApp.Api.Models
{
    public class LocationDetail
    {
        public long Id { get; set; }
        public long LocationId { get; set; }
        public long? ClimateId { get; set; }
        public long CheckId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}