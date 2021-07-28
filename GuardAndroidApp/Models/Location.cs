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
    public class Location
    {
        [PrimaryKey]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Qr { get; set; }
        public string Nfc { get; set; }
        public long GuardAreaId { get; set; }
        
    }
}