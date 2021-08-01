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
    public class AttendanceDetail
    {
        [PrimaryKey]
        public long Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Leave { get; set; }
        public DateTime Date { get; set; }
        public long GuardId { get; set; }
    }
}