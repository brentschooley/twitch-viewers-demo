using Android.App;
using Android.Widget;
using Android.OS;
using TwitchViewersDemo;
using System.Collections.Generic;
using System.Linq;

namespace TwitchViewersDemo.Droid
{
	[Activity (Label = "Twitch Viewers", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		ListView listView;
		ViewersAdapter adapter;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			Xamarin.Insights.Initialize (global::TwitchViewersDemo.Droid.XamarinInsights.ApiKey, this);
			base.OnCreate (savedInstanceState);
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			// Get our button from the layout resource,
			// and attach an event to it
			listView = FindViewById<ListView> (Resource.Id.listView);

			adapter = new ViewersAdapter (this);
			listView.Adapter = adapter;
			adapter.UpdateViewers ();
		}
	}
		
	class ViewersAdapter : BaseAdapter<TwitchViewer>
	{
		public ViewersAdapter(Activity parentActivity)
		{
			activity = parentActivity;
		}

		List<TwitchViewer> viewers = new List<TwitchViewer>();
		Activity activity;
		int modCount;


		public async void UpdateViewers()
		{
			lock (viewers)
			{
				viewers.Clear();
			}

			var mods = await TwitchViewerService.GetMods ();
			modCount = mods.Count;

			viewers = mods;
			viewers.AddRange(await TwitchViewerService.GetViewers());

			activity.RunOnUiThread(() =>
				NotifyDataSetChanged());
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			var view = convertView as LinearLayout ?? activity.LayoutInflater.Inflate(Resource.Layout.ViewerItemLayout, null) as LinearLayout;
			var viewer = viewers[position];

			view.FindViewById<TextView>(Resource.Id.usernameTextView).Text = viewer.Name;
			view.FindViewById<TextView>(Resource.Id.rankTextView).Text = viewer.IsMod ? "moderator" : "";

			return view;
		}

		public override int Count { get { return viewers.Count; } }
		public override TwitchViewer this[int index] { get { return viewers[index]; } }

//		public int GetPositionForSection (int sectionIndex)
//		{
//			if (sectionIndex == 0) {
//				return 0;
//			} else {
//				return modCount;
//			}
//		}
//
//		public int GetSectionForPosition (int position)
//		{
//			if (position < modCount) {
//				return 0;
//			} else {
//				return 1;
//			}
//		}
//
//		public Java.Lang.Object[] GetSections ()
//		{
//			var sectionsObjects = new Java.Lang.Object[2];
//
//			sectionsObjects [0] = new Java.Lang.String ("Moderators");
//			sectionsObjects [1] = new Java.Lang.String ("Viewers");
//
//			return sectionsObjects;
//		}
	}
}
