using Android.App;
using Android.Widget;
using Android.OS;
using TwitchViewersDemo;
using System.Collections.Generic;
using System.Linq;
using AndroidHUD;
using System;
using Android.Views;

using StickyListHeaders;
using Android.Content;

namespace TwitchViewersDemo.Droid
{
	[Activity (Label = "Twitch Viewers", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		
		StickyListHeadersListView listView;
		ViewersAdapter adapter;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			Xamarin.Insights.Initialize (global::TwitchViewersDemo.Droid.XamarinInsights.ApiKey, this);
			base.OnCreate (savedInstanceState);
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			// Get our button from the layout resource,
			// and attach an event to it

			AndHUD.Shared.Show(this, "Loading viewers...", -1, MaskType.Clear, TimeSpan.FromSeconds(10));

			listView = FindViewById<StickyListHeadersListView> (Resource.Id.listView);

			adapter = new ViewersAdapter (this);
			listView.Adapter = adapter;
			adapter.UpdateViewers ();
		}
	}
		
	class ViewersAdapter : BaseAdapter<TwitchViewer>, IStickyListHeadersAdapter, ISectionIndexer
	{
		public ViewersAdapter(Activity activity)
		{
			this.activity = activity;
			//activity.contex
			inflater = LayoutInflater.From(activity.BaseContext);
		}

		private int[] GetSectionIndices()
		{
			var sections = new int[2];
			sections [0] = 0;
			sections [1] = modCount;
			return sections;
		}

		List<TwitchViewer> viewers = new List<TwitchViewer>();
		Activity activity;
		//Context context;
		int modCount;
		private readonly LayoutInflater inflater;
		private int[] sectionIndices;

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

			activity.RunOnUiThread(() => {
				NotifyDataSetChanged();
				AndHUD.Shared.Dismiss(activity);
			});

			sectionIndices = GetSectionIndices();
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			var view = convertView as LinearLayout ?? activity.LayoutInflater.Inflate(Resource.Layout.ViewerItemLayout, null) as LinearLayout;
			var viewer = viewers[position];

			view.FindViewById<TextView>(Resource.Id.usernameTextView).Text = viewer.Name;
			view.FindViewById<TextView>(Resource.Id.rankTextView).Text = viewer.IsMod ? "moderator" : "";

			return view;
		}

		public override int Count { get { return viewers.Count; } }
		public override TwitchViewer this[int index] { get { return viewers[index]; } }


		public long GetHeaderId (int position)
		{
			if (position < modCount) {
				return 'm';
			} else {
				return 'v';
			}
		}

		public View GetHeaderView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			ViewHolder holder;

			if (convertView == null)
			{
				holder = new ViewHolder();
				convertView = inflater.Inflate(Resource.Layout.Header, parent, false);
				holder.text = convertView.FindViewById<TextView>(Resource.Id.text1);
				convertView.Tag = holder;
			}
			else
			{
				holder = (ViewHolder)convertView.Tag;
			}

			// set header text as first char in name
			string headerString;

			if (position == 0) {
				// Moderator section
				headerString = $"Moderators - {modCount}";
			} else {
				// Viewers section
				headerString = $"Viewers - {viewers.Count - modCount}";
			}

			holder.text.Text = headerString;

			return convertView;
		}

		public int GetPositionForSection (int section)
		{
			if (sectionIndices.Length == 0)
			{
				return 0;
			}
			if (section >= sectionIndices.Length)
			{
				section = sectionIndices.Length - 1;
			}
			else if (section < 0)
			{
				section = 0;
			}
			return sectionIndices[section];
		}

		public int GetSectionForPosition (int position)
		{
			for (int i = 0; i < sectionIndices.Length; i++)
			{
				if (position < sectionIndices[i])
				{
					return i - 1;
				}
			}
			return sectionIndices.Length - 1;
		}

		public Java.Lang.Object[] GetSections ()
		{
			var sections = new Java.Lang.String[2];
			sections [0] = new Java.Lang.String("Moderators");
			sections [1] = new Java.Lang.String("Viewers");
			return sections;
		}

		private class ViewHolder : Java.Lang.Object
		{
			public TextView text;
		}
	}
}
