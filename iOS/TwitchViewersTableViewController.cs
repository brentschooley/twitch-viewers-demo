using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
//using System.Json;
using BigTed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchViewersDemo.iOS
{
	partial class TwitchViewersTableViewController : UITableViewController, IUITableViewDataSource, IUITableViewDelegate
	{
		public List<TwitchViewer> Viewers {
			get;
			set;
		}

		public List<TwitchViewer> Mods
		{
			get;
			set;
		}

		public long? ViewerCount {
			get;
			set;
		}

		public TwitchViewersTableViewController (IntPtr handle) : base (handle)
		{
			Viewers = new List<TwitchViewer>();
			Mods = new List<TwitchViewer>();
		}

		public async override void ViewDidLoad()
		{
			base.ViewDidLoad();

			BTProgressHUD.ForceiOS6LookAndFeel = true;
			BTProgressHUD.Show("Loading viewers...");

			Viewers = await TwitchViewerService.GetViewers();
			Mods = await TwitchViewerService.GetMods();
			ViewerCount = await TwitchViewerService.GetViewerCount ();
			//TwitchViewerService.TestFollowersMethod ();

			if(ViewerCount.HasValue)
			{
				this.Title = $"Twitch Viewers - {ViewerCount.Value} online";
			}

			this.TableView.ReloadData();

			BTProgressHUD.Dismiss();
		}

		//public async Task GetViewers()
		//{
		//	var http = new HttpClient();
		//	var viewersJson = await http.GetStringAsync("https://tmi.twitch.tv/group/user/brentschooley/chatters");

		//	var json = JObject.Parse(viewersJson);
		//	var mods = json["chatters"]["moderators"];

		//	foreach (var mod in mods)
		//	{
		//		var twitchViewer = new TwitchViewer() { Name = mod.ToString(), IsMod = true };
		//		this.Mods.Add(twitchViewer);
		//	}

		//	var viewers = json["chatters"]["viewers"];
		//	foreach (var viewer in viewers)
		//	{
		//		var twitchViewer = new TwitchViewer() { Name = viewer.ToString(), IsMod = false };
		//		this.Viewers.Add(twitchViewer);
		//	}
		//}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 2;
		}

		public override string TitleForHeader(UITableView tableView, nint section)
		{
			return section == 0 ? $"Moderators - {Mods.Count}" : $"Viewers - {Viewers.Count}";
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return section == 0 ? Mods.Count : Viewers.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = this.TableView.DequeueReusableCell ("TwitchViewerCell");
			TwitchViewer viewer;

			if (indexPath.Section == 0)
			{
				viewer = Mods[indexPath.Row];
			}
			else
			{
				viewer = Viewers[indexPath.Row];
			}

			cell.TextLabel.Text = viewer.Name;
			cell.DetailTextLabel.Text = viewer.IsMod ? "moderator" : "";
			return cell;
		}
	}
}
