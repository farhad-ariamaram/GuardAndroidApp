using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuardAndroidApp.Api.Models
{
    public class User
    {
        public string Status;
        public string id;
        public string name;
        public string IsGuard;
        public string IsGuardAdmin;
        public string IsEmployeeRequest;
        public string IsGuardRecorder;
        public string IsMould;
        public string token;
    }
}