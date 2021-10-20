using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GuardAndroidApp.Api;
using GuardAndroidApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GuardAndroidApp
{
    [Service]
    public class SyncService : Service
    {
        DbContext _db;
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            _db = new DbContext();

            CreateNotificationChannel();
            string messageBody = "service starting";



            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                //Android.App.Application.Context.StartForegroundService(intent);
                var notification = new Notification.Builder(this, "10111")
                .SetContentTitle("Foreground")
                .SetContentText(messageBody)
                .SetOngoing(true)
                .Build();
                StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
            }
            else
            {
                Android.App.Application.Context.StartService(intent);
            }



            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(60);

            var timer = new Timer(Timer_Callback, null, startTimeSpan, periodTimeSpan);

            return StartCommandResult.Sticky;
        }

        private async void Timer_Callback(object state)
        {

            try
            {

                if (await IsConnect())
                {
                    var allAsyncSubLocs = _db.SubmittedLocationsList().Where(a => !a.IsSync).ToList();
                    foreach (var item in allAsyncSubLocs)
                    {
                        var res = await ApiRepository.postSubmittedLocations(item);

                        if (res)
                        {
                            _db.IsSyncTrue(item);
                        }
                        else
                        {
                            //Toast.MakeText(Application.Context, "مشکل در همگام سازی با سرور", ToastLength.Long).Show();
                        }
                    }

                    await Task.Delay(1000);

                    var allAsyncSubLocDtls = _db.SubmittedLocationDtlsList().Where(a => !a.IsSync).ToList();
                    foreach (var item in allAsyncSubLocDtls)
                    {
                        if (_db.StatusById(item.SubmittedLocationId))
                        {
                            var res = await ApiRepository.postSubmittedLocationDtls(item);
                            if (res)
                            {
                                _db.SubmittedLocationDtlIsSyncTrue(item);
                            }
                            else
                            {
                                //Toast.MakeText(Application.Context, "تایمر", ToastLength.Long).Show();
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //Toast.MakeText(Application.Context, "مشکل در همگام سازی با سرور", ToastLength.Long).Show();
            }

        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            var channelName = "Guard";
            var channelDescription = "Guard Android App";
            var channel = new NotificationChannel("10111", channelName, NotificationImportance.Default)
            {
                Description = channelDescription
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
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
    }
}