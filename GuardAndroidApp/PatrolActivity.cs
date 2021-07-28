using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using GuardAndroidApp.Adapters;
using GuardAndroidApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZXing.Mobile;

namespace GuardAndroidApp
{
    [Activity(Label = "گشت زنی", Theme = "@android:style/Theme.Material.Light")]
    public class PatrolActivity : Activity
    {
        Button scanButton;
        //ISharedPreferences prefs;
        ListView plansLV;
        DbContext _db;
        List<Plan> currentPlanningList = null;
        Timer timer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.PatrolLayout);

            if (_db.GetLogin() == null)
            {
                StartActivity(typeof(LoginActivity));
            }

            //prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            scanButton = FindViewById<Button>(Resource.Id.ScanButton);
            _db = new DbContext();

            scanButton.Click += ScanButton_Click;

            currentPlanningList = _db.GetPlansList();

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
                        //UserID = Int64.Parse(prefs.GetString("uid", null)),
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