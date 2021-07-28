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

namespace GuardAndroidApp.Api.Models
{
    public class SubmittedLocationDtl
    {
        public long Id { get; set; }
        public long SubmittedLocationId { get; set; }
        public long LocationDetailId { get; set; }
        public long RunStatusId { get; set; }
    }
}