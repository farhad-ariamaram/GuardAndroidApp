using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GuardAndroidApp.Utilities
{
    public static class Utils
    {
        public const string _SALT = "SALT";

        public static string sha512(string strToEncrypt)
        {
            UTF8Encoding ue = new UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);
            SHA512CryptoServiceProvider sha512 = new SHA512CryptoServiceProvider();
            byte[] hashBytes = sha512.ComputeHash(bytes);
            return System.Text.RegularExpressions.Regex.Replace(BitConverter.ToString(hashBytes), "-", "").ToLower();
        }

        public static string toPersianDate(this DateTime datetime)
        {
            if (datetime == null)
            {
                return "";
            }
            PersianCalendar pc = new PersianCalendar();
            DateTime date = (DateTime)datetime;
            return string.Format("{0}/{1}/{2} {3}", pc.GetYear(date), pc.GetMonth(date), pc.GetDayOfMonth(date), datetime.ToString("HH:mm"));
        }

        public static string toPersianDayOfWeek(this DateTime datetime)
        {
            if (datetime == null)
            {
                return "";
            }
            switch (datetime.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "یکشنبه";
                case DayOfWeek.Monday:
                    return "دوشنبه";
                case DayOfWeek.Tuesday:
                    return "سشنبه";
                case DayOfWeek.Wednesday:
                    return "چهارشنبه";
                case DayOfWeek.Thursday:
                    return "پنجشنبه";
                case DayOfWeek.Friday:
                    return "جمعه";
                case DayOfWeek.Saturday:
                    return "شنبه";
                default:
                    return "";
            }
        }

        public static string toTimeOfDay(this DateTime datetime)
        {
            if (datetime == null)
            {
                return "";
            }

            if (datetime.Hour >= 0 && datetime.Hour < 5)
            {
                return "بامداد";
            }
            else if (datetime.Hour >= 5 && datetime.Hour < 12)
            {
                return "صبح";
            }
            else if (datetime.Hour >= 12 && datetime.Hour < 17)
            {
                return "ظهر";
            }
            else if (datetime.Hour >= 17 && datetime.Hour < 20)
            {
                return "عصر";
            }
            else
            {
                return "شب";
            }

        }
    }
}