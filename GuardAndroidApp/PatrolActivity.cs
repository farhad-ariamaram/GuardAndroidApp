using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using GuardAndroidApp.Adapters;
using GuardAndroidApp.Api;
using GuardAndroidApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZXing.Mobile;

namespace GuardAndroidApp
{
    [Activity(Label = "گشت زنی", Theme = "@android:style/Theme.Material.Light", NoHistory = true)]
    public class PatrolActivity : Activity
    {
        Button scanButton;
        ListView plansLV;
        DbContext _db;
        List<Plan> currentPlanningList = null;
        Timer timer;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.PatrolLayout);

            _db = new DbContext();

            if (_db.GetLogin() == null)
            {
                StartActivity(typeof(LoginActivity));
            }

            scanButton = FindViewById<Button>(Resource.Id.ScanButton);

            scanButton.Click += ScanButton_Click;

            //get guard id
            var guardId = _db.GetLogin().Id;

            //get datetime
            var datetime = DateTime.Now;

            //GetAttendanceDetails(date,uid) if was empty get from api
            var att = _db.GetAttendanceDetails(datetime, guardId);
            if (att.Count < 1)
            {
                try
                {
                    var attapi = await ApiRepository.getAttendant(guardId, datetime);
                    foreach (var item in attapi)
                    {
                        _db.InsertAttendanceDetail(new AttendanceDetail
                        {
                            Id=item.Id,
                            Date = item.Date,
                            EndDate = item.EndDate,
                            StartDate = item.StartDate,
                            GuardId = item.GuardId,
                            Leave = item.Leave
                        });
                    }
                }
                catch (Exception)
                {
                    Toast.MakeText(Application.Context, "مشکل در دریافت شیفت کاری نگهبان از سرور", ToastLength.Long).Show();
                }

            }


            ///////////////*******************مانده******************///////////////////////////////////
            //filter currentPlanningList based on AttendanceDetails
            currentPlanningList = _db.GetPlansList();
            ///////////////*******************مانده******************///////////////////////////////////


            if (currentPlanningList.Any())
            {
                var startTimeSpan = TimeSpan.Zero;
                var periodTimeSpan = TimeSpan.FromSeconds(10);

                timer = new Timer((e) =>
                {
                    RunOnUiThread(() => refreshList());

                }, null, startTimeSpan, periodTimeSpan);
            }
            else
            {
                Toast.MakeText(Application.Context, "لیست پلن ها خالی است یا به دلیل عدم اتصال به شبکه دریافت نشده است", ToastLength.Long).Show();
            }

        }

        protected async override void OnDestroy()
        {
            base.OnDestroy();
            await disposeTimer();
        }

        private async Task disposeTimer()
        {
            await timer.DisposeAsync();
        }

        private void refreshList()
        {
            plansLV = FindViewById<ListView>(Resource.Id.plansListView);

            plansLV.Adapter = new PlanAdapter(this, currentPlanningList);
        }

        private async void ScanButton_Click(object sender, EventArgs e)
        {
            MobileBarcodeScanner.Initialize(Application);

            var scanner = new MobileBarcodeScanner();

            var result = await scanner.Scan();

            if (result != null)
            {
                try
                {
                    long locId = _db.GetIdFromQR(result.Text);
                    SubmittedLocation subloc = new SubmittedLocation()
                    {
                        DateTime = DateTime.Now,
                        DeviceId = 1,
                        IsSync = false,
                        LocationId = locId,
                        UserId = _db.GetLogin().Id
                    };

                    _db.InsertSubmittedLocation(subloc);
                    //playSound("submitBarcodeSuccess");

                    Intent i = new Intent(this, typeof(LocationDetailActivity));
                    i.PutExtra("LocationId", locId);
                    i.PutExtra("SubmittedLocationId", subloc.Id);
                    StartActivity(i);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(Application.Context, "مشکل در ثبت بارکد", ToastLength.Long).Show();
                    //playSound("submitBarcodeFailed");
                }
            }

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}