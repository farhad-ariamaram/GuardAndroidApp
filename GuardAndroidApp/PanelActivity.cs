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
    [Activity(Label = "پنل کاربری", Theme = "@android:style/Theme.Material.Light", NoHistory = true)]
    public class PanelActivity : Activity
    {
        DbContext _db;
        Button patrolButton, logoutButton;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.PanelLayout);

            _db = new DbContext();

            var login = _db.GetLogin();

            if (login == null)
            {
                StartActivity(typeof(LoginActivity));
            }

            patrolButton = FindViewById<Button>(Resource.Id.patrolBTN);
            logoutButton = FindViewById<Button>(Resource.Id.logoutBTN);

            patrolButton.Click += PatrolButton_Click;
            logoutButton.Click += LogoutButton_Click;

        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            _db.ClearLogin();
            StartActivity(typeof(LoginActivity));
        }

        private void PatrolButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(PatrolActivity));
        }
    }
}