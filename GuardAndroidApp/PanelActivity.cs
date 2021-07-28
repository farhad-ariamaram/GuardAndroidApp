using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GuardAndroidApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuardAndroidApp
{
    [Activity(Label = "پنل کاربری", Theme = "@android:style/Theme.Material.Light")]
    public class PanelActivity : Activity
    {
        DbContext _db;
        Button patrolButton;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.PanelLayout);

            _db = new DbContext();
            patrolButton = FindViewById<Button>(Resource.Id.patrolBTN);

            patrolButton.Click += PatrolButton_Click;


            //0000000000000000000000000000test

            TextView tv = FindViewById<TextView>(Resource.Id.textView1);
            var a = _db.GetPlansList();
            var b = _db.GetLocationsList();
            var c = _db.GetCheckList();
            var d = _db.GetClimateList();
            var e = _db.GetLocationDetailList();

            foreach (var item in a)
            {
                tv.Text += $"plan:{item.DateTime}\n\r";
            }

            foreach (var item in b)
            {
                tv.Text += $"location:{item.Name}\n\r";
            }

            foreach (var item in c)
            {
                tv.Text += $"check:{item.Name}\n\r";
            }

            foreach (var item in d)
            {
                tv.Text += $"climate:{item.Name}\n\r";
            }

            foreach (var item in e)
            {
                tv.Text += $"locationList:{item.StartTime.Value.ToString(@"hh\:mm\:ss")}\n\r";
            }



            //await Api.ApiRepository.postSubmittedLocations(new SubmittedLocation
            //{
            //    DateTime = DateTime.Now,
            //    DeviceId = 1,
            //    IsSync = true,
            //    LocationId = 20005,
            //    UserId = 199
            //});
            //test
        }

        private void PatrolButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(PatrolActivity));
        }
    }
}