
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GuardAndroidApp;
using GuardAndroidApp.Utilities;
using GuardAndroidApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuardAndroidApp.Adapters
{
    class PlanAdapter : BaseAdapter<Plan>
    {
        private Activity _context;
        private List<Plan> _list;
        private DbContext _db;

        public PlanAdapter(Activity context, List<Plan> list)
        {
            _context = context;
            _list = list;
            _db = new DbContext();
        }

        public override Plan this[int position]
        {
            get { return _list[position]; }
        }

        public override int Count
        {
            get { return _list.Count(); }
        }

        public override long GetItemId(int position)
        {
            return _list[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            if (view == null)
            {
                view = _context.LayoutInflater.Inflate(Resource.Layout.PlanListView, null);
            }

            string placename = _db.GetLocationName(int.Parse(_list[position].LocationId+""));

            view.FindViewById<TextView>(Resource.Id.placeNameTextView).Text = placename;
            view.FindViewById<TextView>(Resource.Id.DateTextView).Text = _list[position].DateTime.toPersianDate();
            view.FindViewById<TextView>(Resource.Id.dayOfWeekTextView).Text = _list[position].DateTime.toPersianDayOfWeek() + " " + _list[position].DateTime.toTimeOfDay();

            var test = DateTime.Now.Add(new TimeSpan(0, 5, 0));
            if (_list[position].DateTime < DateTime.Now)
            {
                view.FindViewById<LinearLayout>(Resource.Id.linearLayout9).SetBackgroundColor(Color.Argb(65, 100, 255, 0));
            }
            else if (_list[position].DateTime > DateTime.Now && _list[position].DateTime <= test)
            {
                view.FindViewById<LinearLayout>(Resource.Id.linearLayout9).SetBackgroundColor(Color.Argb(65, 255, 0, 0));
            }
            else
            {
                view.FindViewById<LinearLayout>(Resource.Id.linearLayout9).SetBackgroundColor(Color.Argb(0, 255, 255, 255));
            }

            return view;
        }
    }
}