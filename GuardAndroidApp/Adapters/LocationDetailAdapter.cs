using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GuardAndroidApp.Models;
using System;
using System.Collections.Generic;
using GuardAndroidApp.Utilities;
using System.Linq;
using System.Text;
using Android.Graphics;

namespace GuardAndroidApp.Adapters
{
    public class LocationDetailAdapter : BaseAdapter<LocationDetail>
    {
        private DbContext _db;
        private Activity _Context;
        private List<LocationDetail> _List;

        public LocationDetailAdapter(Activity Context, List<LocationDetail> List)
        {
            _db = new DbContext();
            _Context = Context;
            _List = List;
        }

        public override LocationDetail this[int position] { get { return _List[position]; } }

        public override int Count { get { return _List.Count; } }

        public override long GetItemId(int position)
        {
            return _List[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
            {
                view = _Context.LayoutInflater.Inflate(Resource.Layout.LocationDetailListView, null);
            }

            view.FindViewById<TextView>(Resource.Id.checkNameTextView).Text = $"id: {_List[position].Id} -  check: {_db.GetCheckById(_List[position].CheckId).Name}" ;

            return view;

        }

    }
}