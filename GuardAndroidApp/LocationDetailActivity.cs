using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GuardAndroidApp.Adapters;
using GuardAndroidApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Android.Widget.AdapterView;

namespace GuardAndroidApp
{
    [Activity(Label = "جزئیات مکان", Theme = "@android:style/Theme.Material.Light", NoHistory = true)]
    public class LocationDetailActivity : Activity
    {
        ListView LocationDetailList;
        Button backButton;
        DbContext _db;
        List<LocationDetail> LocationDtlList;
        bool flag = true;
        bool flag2 = true;
        long LocationId;
        long SubmittedLocationId;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LocationDetailLayout);

            if (Intent.HasExtra("LocationId") && Intent.HasExtra("SubmittedLocationId"))
            {
                LocationId = Intent.GetLongExtra("LocationId", 0);
                SubmittedLocationId = Intent.GetLongExtra("SubmittedLocationId", 0);

                if (Intent.GetLongExtra("LocationId", 0) == 0 || Intent.GetLongExtra("SubmittedLocationId", 0) == 0)
                {
                    StartActivity(typeof(PatrolActivity));
                }
            }
            else
            {
                StartActivity(typeof(PatrolActivity));
            }

            LocationDetailList = FindViewById<ListView>(Resource.Id.LocationDetailList);
            _db = new DbContext();
            backButton = FindViewById<Button>(Resource.Id.backButton);
            

            backButton.Click += BackButton_Click;

            LocationDtlList = _db.GetLocationDetailList().Where(a => a.LocationId == LocationId).ToList();

            LocationDetailList.Adapter = new LocationDetailAdapter(this, LocationDtlList);

            LocationDetailList.ItemClick += LocationDetailList_ItemClick;

        }

        private void LocationDetailList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (flag2)
            {
                flag2 = false;

                LocationDetail ld = _db.GetLocationDetailById(e.Id);

                if (LocationDetailList.GetChildAt(e.Position).Alpha == (float)0.9)
                {
                    LocationDetailList.GetChildAt(e.Position).Alpha = (float)0.8;

                    LocationDetailList.GetChildAt(e.Position).SetBackgroundColor(Color.Red);

                    _db.UnCheckLocationDetail(ld);

                }
                else
                {
                    LocationDetailList.GetChildAt(e.Position).Alpha = (float)0.9;

                    LocationDetailList.GetChildAt(e.Position).SetBackgroundColor(Color.Green);

                    _db.CheckLocationDetail(ld);

                }

                LocationDtlList = _db.GetLocationDetailList().Where(a => a.LocationId == LocationId).ToList();

                flag2 = true;
            }

        }



        private void BackButton_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < LocationDetailList.ChildCount; i++)
            {
                var alpha = LocationDetailList.GetChildAt(i).Alpha;
                if (alpha != (float)0.9 && alpha != (float)0.8)
                {
                    flag = false;
                }
            }

            if (flag)
            {
                LocationDtlList = _db.GetLocationDetailList().Where(a => a.LocationId == LocationId).ToList();

                foreach (var item in LocationDtlList)
                {
                    _db.InsertSubmittedLocationDtl(new SubmittedLocationDtl { 
                        IsSync = false,
                        LocationDetailId = item.Id,
                        SubmittedLocationId = SubmittedLocationId,
                        RunStatusId = (item.Check) ? 1 : 2
                    });
                }

                StartActivity(typeof(PatrolActivity));
            }
            else
            {
                flag = true;
                Toast.MakeText(Application.Context, "ابتدا همه موارد را بررسی کنید", ToastLength.Long).Show();
            }

        }
    }
}