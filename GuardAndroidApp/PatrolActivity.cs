using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Nfc;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using GuardAndroidApp.Adapters;
using GuardAndroidApp.Api;
using GuardAndroidApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZXing.Mobile;

namespace GuardAndroidApp
{
    [Activity(Label = "گشت زنی", Theme = "@android:style/Theme.Material.Light", NoHistory = true)]
    [IntentFilter(new[] { NfcAdapter.ActionNdefDiscovered, NfcAdapter.ActionTagDiscovered, Intent.CategoryDefault })]
    public class PatrolActivity : Activity
    {
        Button scanButton;
        ListView plansLV;
        DbContext _db;
        List<Plan> currentPlanningList = null;
        Timer timer;
        List<Plan> filteredPlanningList = new List<Plan>();
        private NfcAdapter _nfcAdapter;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.PatrolLayout);

            _db = new DbContext();

            if (_db.GetLogin() == null)
            {
                StartActivity(typeof(LoginActivity));
            }

            _nfcAdapter = NfcAdapter.GetDefaultAdapter(this);

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
                            Id = item.Id,
                            Date = item.Date,
                            EndDate = item.EndDate,
                            StartDate = item.StartDate,
                            GuardId = item.GuardId,
                            Leave = item.Leave
                        });
                    }
                    att = _db.GetAttendanceDetails(datetime, guardId);
                }
                catch (Exception e)
                {
                    Toast.MakeText(Application.Context, "مشکل در دریافت شیفت کاری نگهبان از سرور", ToastLength.Long).Show();
                }

            }

            currentPlanningList = _db.GetPlansList().Where(a => a.UserId == guardId).OrderBy(a=>a.DateTime).ToList();
            

            foreach (var item in currentPlanningList)
            {
                bool flag = false;
                foreach (var item2 in att)
                {
                    if (item.DateTime >= item2.StartDate && item.DateTime <= item2.EndDate && item2.Leave == false)
                    {
                        flag = true;
                    }
                    else if(item.DateTime >= item2.StartDate && item.DateTime <= item2.EndDate && item2.Leave == true)
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    filteredPlanningList.Add(item);
                }
            }

            

            if (filteredPlanningList.Any())
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

        protected override void OnResume()
        {
            base.OnResume();

            if (_nfcAdapter == null)
            {
                var alert = new Android.App.AlertDialog.Builder(this).Create();
                alert.SetMessage("NFC is not supported on this device.");
                alert.SetTitle("NFC Unavailable");
                alert.Show();
            }
            else
            {
                //Set events and filters
                var tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
                var ndefDetected = new IntentFilter(NfcAdapter.ActionNdefDiscovered);
                var techDetected = new IntentFilter(NfcAdapter.ActionTechDiscovered);
                var filters = new[] { ndefDetected, tagDetected, techDetected };

                var intent = new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop);

                var pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);

                // Gives your current foreground activity priority in receiving NFC events over all other activities.
                _nfcAdapter.EnableForegroundDispatch(this, pendingIntent, filters, null);
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            if (intent.Action != NfcAdapter.ActionTagDiscovered) return;
            var myTag = (Tag)intent.GetParcelableExtra(NfcAdapter.ExtraTag);
            if (myTag == null) return;
            var tagIdBytes = myTag.GetId();
            var tagIdString = ByteArrayToString(tagIdBytes); //Byte array converted to string
            var reverseHex = LittleEndian(tagIdString); // Reversed hex converted to hex
            var cardId = Convert.ToInt64(reverseHex, 16); //Convert to decimal decimal to get the final value
            var alertMessage = new Android.App.AlertDialog.Builder(this).Create();
            alertMessage.SetMessage("CardId:" + cardId); // Here's the id of card
            alertMessage.Show();
            if (myTag != null)
            {
                try
                {
                    long locId = _db.GetIdFromNFC(cardId + "");
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


            //// Extra: Check if there's any Ndef message
            //var rawMessages = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
            //if (rawMessages == null) return;
            //var msg = (NdefMessage)rawMessages[0];
            //// Get NdefRecord which contains the actual data
            //var record = msg.GetRecords()[0];
            //if (record == null) return;
            //// The data is defined by the Record Type Definition (RTD) specification available from http://members.nfc-forum.org/specs/spec_list/
            //if (record.Tnf != NdefRecord.TnfWellKnown) return;
            //// Get the transmitted data
            //var data = Encoding.ASCII.GetString(record.GetPayload());
            ////alertMessage.SetMessage("Data:" + data);

        }

        private static string LittleEndian(string num)
        {
            var number = Convert.ToInt32(num, 16);
            var bytes = BitConverter.GetBytes(number);
            return bytes.Aggregate("", (current, b) => current + b.ToString("X2"));
        }

        public static string ByteArrayToString(byte[] ba)
        {
            var shb = new SoapHexBinary(ba);
            return shb.ToString();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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

            plansLV.Adapter = new PlanAdapter(this, filteredPlanningList);
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

        //public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        //{
        //    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        //    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        //}
    }
}