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
using System.Threading.Tasks;

namespace GuardAndroidApp
{
    [Activity(Label = "ورود به سیستم", Theme = "@android:style/Theme.Material.Light", NoHistory = true)]
    public class LoginActivity : Activity
    {
        DbContext _db;
        Button loginButton;
        EditText usernameET, passwordET;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LoginLayout);

            if (_db.GetLogin() != null)
            {
                StartActivity(typeof(PanelActivity));
            }

            _db = new DbContext();
            loginButton = FindViewById<Button>(Resource.Id.loginBTN);
            usernameET = FindViewById<EditText>(Resource.Id.usernameET);
            passwordET = FindViewById<EditText>(Resource.Id.passwordET);

            loginButton.Click += LoginButton_Click;
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            loginButton.Enabled = false;
            try
            {
                var result = await ApiRepository.RemoteLogin(usernameET.Text, passwordET.Text);
                if (string.IsNullOrEmpty(result.id))
                {
                    var resultLocal = _db.LocalLogin(usernameET.Text, passwordET.Text);
                    if (resultLocal == null)
                    {
                        Toast.MakeText(Application.Context, "نام کاربری یا کلمه عبور اشتباه است", ToastLength.Long).Show();
                    }
                    else
                    {
                        _db.AddLogin(new Login
                        {
                            Id = resultLocal.Id,
                            Name = resultLocal.Name,
                            Username = resultLocal.Username
                        });
                        StartActivity(typeof(PanelActivity));
                    }
                }
                else
                {
                    _db.InsertUser(new User
                    {
                        Id = int.Parse(result.id),
                        Name = result.name,
                        Password = passwordET.Text,
                        Token = result.token,
                        Username = usernameET.Text,
                        UserTypeId = 1
                    });

                    _db.AddLogin(new Login
                    {
                        Id = int.Parse(result.id),
                        Name = result.name,
                        Username = usernameET.Text
                    });

                    StartActivity(typeof(PanelActivity));
                }
            }
            catch (Exception)
            {
                var resultLocal = _db.LocalLogin(usernameET.Text, passwordET.Text);
                if (resultLocal == null)
                {
                    Toast.MakeText(Application.Context, "نام کاربری یا کلمه عبور اشتباه است", ToastLength.Long).Show();
                }
                else
                {
                    _db.AddLogin(new Login
                    {
                        Id = resultLocal.Id,
                        Name = resultLocal.Name,
                        Username = resultLocal.Username
                    });
                    StartActivity(typeof(PanelActivity));
                }
            }
            finally
            {
                loginButton.Enabled = true;
            }
        }
    }
}