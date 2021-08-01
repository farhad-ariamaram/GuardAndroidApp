using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using GuardAndroidApp.Api;
using GuardAndroidApp.Models;
using System;
using System.Threading.Tasks;

namespace GuardAndroidApp
{
    [Activity(Label = "نگهبان", MainLauncher = true, Theme = "@android:style/Theme.Light.NoTitleBar.Fullscreen", NoHistory = true)]
    public class SplashActivity : Activity
    {
        DbContext _db;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SplashLayout);

            _db = new DbContext();

            var sync = await SyncStatic();
            if (!sync)
            {
                Toast.MakeText(Application.Context, "مشکل در همگام سازی با سرور", ToastLength.Long).Show();
            }

            var intent = new Intent(this, typeof(SyncService));
            StartService(intent);

            var login = _db.GetLogin();

            if (login == null)
            {
                StartActivity(typeof(LoginActivity));
            }
            else
            {
                StartActivity(typeof(PanelActivity));
            }
            
        }

        private async Task<bool> IsConnect()
        {
            try
            {
                var isConnect = await ApiRepository.CheckConnectivity();
                if (isConnect)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> SyncStatic()
        {
            try
            {
                if (await IsConnect())
                {
                    var locations = await ApiRepository.getLocations();
                    var plans = await ApiRepository.getPlans();
                    var checks = await ApiRepository.getChecks();
                    var climates = await ApiRepository.getClimate();
                    var locationDetails = await ApiRepository.getLocationDetail();

                    foreach (var item in locations)
                    {
                        var loc = new GuardAndroidApp.Models.Location
                        {
                            GuardAreaId = item.GuardAreaId,
                            Id = item.Id,
                            Name = item.Name,
                            Nfc = item.Nfc,
                            Qr = item.Qr
                        };
                        _db.InsertLocation(loc);
                    }

                    foreach (var item in plans)
                    {
                        var pln = new GuardAndroidApp.Models.Plan
                        {
                            Id = item.Id,
                            DateTime = item.DateTime,
                            LocationId = item.LocationId,
                            ShiftId = item.ShiftId,
                            UserId = item.UserId
                        };
                        _db.InsertPlan(pln);
                    }

                    foreach (var item in checks)
                    {
                        var chk = new GuardAndroidApp.Models.Check
                        {
                            Id = item.Id,
                            Name = item.Name
                        };
                        _db.InsertCheck(chk);
                    }

                    foreach (var item in climates)
                    {
                        var cli = new GuardAndroidApp.Models.Climate
                        {
                            Id = item.Id,
                            Name = item.Name
                        };
                        _db.InsertClimate(cli);
                    }

                    foreach (var item in locationDetails)
                    {
                        var locdtl = new GuardAndroidApp.Models.LocationDetail
                        {
                            Id = item.Id,
                            CheckId = item.CheckId,
                            ClimateId = item.ClimateId,
                            LocationId = item.LocationId,
                            EndDate = item.EndDate,
                            EndTime = item.EndTime,
                            StartDate = item.StartDate,
                            StartTime = item.StartTime,
                            Check = false
                        };
                        _db.InsertLocationDetail(locdtl);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}