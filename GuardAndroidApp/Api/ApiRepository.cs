using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GuardAndroidApp.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace GuardAndroidApp.Api
{
    public static class ApiRepository
    {
        private static string Url = "http://192.168.10.247:5000/api/Users/";

        public async static Task<bool> CheckConnectivity()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string data = await client.GetStringAsync(Url + "connect");
                    return JsonConvert.DeserializeObject<bool>(data);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async static Task<User> RemoteLogin(string username, string password)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string data = await client.GetStringAsync(Url + $"login?username={username}&password={password}");
                    return JsonConvert.DeserializeObject<User>(data);
                }
            }
            catch (Exception)
            {
                throw new Exception("Failed to connect API");
            }

        }

        public async static Task<List<Plan>> getPlans()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string data = await client.GetStringAsync(Url + "getPlans");
                    return JsonConvert.DeserializeObject<List<Plan>>(data);
                }
            }
            catch (Exception)
            {
                throw new Exception("Failed to connect API");
            }
        }

        public async static Task<List<Location>> getLocations()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string data = await client.GetStringAsync(Url + "getLocations");
                    return JsonConvert.DeserializeObject<List<Location>>(data);
                }
            }
            catch (Exception)
            {
                throw new Exception("Failed to connect API");
            }
        }

        public async static Task<List<Check>> getChecks()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string data = await client.GetStringAsync(Url + "getChecks");
                    return JsonConvert.DeserializeObject<List<Check>>(data);
                }
            }
            catch (Exception)
            {
                throw new Exception("Failed to connect API");
            }
        }

        public async static Task<List<Climate>> getClimate()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string data = await client.GetStringAsync(Url + "getClimates");
                    return JsonConvert.DeserializeObject<List<Climate>>(data);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to connect API");
            }
        }

        public async static Task<List<LocationDetail>> getLocationDetail()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string data = await client.GetStringAsync(Url + "getLocationDetails");
                    return JsonConvert.DeserializeObject<List<LocationDetail>>(data);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to connect API");
            }
        }

        public async static Task<bool> postSubmittedLocations(GuardAndroidApp.Models.SubmittedLocation submitted)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var payload = new Api.Models.SubmittedLocation
                    {
                        DateTime = submitted.DateTime,
                        DeviceId = submitted.DeviceId,
                        Id = submitted.Id,
                        LocationId = submitted.LocationId,
                        UserId = submitted.UserId
                    };
                    string stringPayload = JsonConvert.SerializeObject(payload);
                    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(Url + "postSubmittedLocations", httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {

                throw new Exception("Failed to connect API");
            }
        }

        public async static Task<bool> postSubmittedLocationDtls(GuardAndroidApp.Models.SubmittedLocationDtl submitted)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var payload = new Api.Models.SubmittedLocationDtl
                    {
                        Id = submitted.Id,
                        LocationDetailId = submitted.LocationDetailId,
                        RunStatusId = submitted.RunStatusId,
                        SubmittedLocationId = submitted.SubmittedLocationId
                    };
                    string stringPayload = JsonConvert.SerializeObject(payload);
                    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(Url + "postSubmittedLocationDtls", httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {

                throw new Exception("Failed to connect API");
            }
        }
    }
}