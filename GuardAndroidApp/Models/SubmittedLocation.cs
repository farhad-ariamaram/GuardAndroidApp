using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuardAndroidApp.Models
{
    public class SubmittedLocation
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public long UserId { get; set; }
        public long LocationId { get; set; }
        public DateTime DateTime { get; set; }
        public long DeviceId { get; set; }
        public bool IsSync { get; set; }
    }
}