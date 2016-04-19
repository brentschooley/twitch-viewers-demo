using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using TwixelAPI;

namespace TwitchViewersDemo
{
	public static class TwitchViewerService
	{
		// https://tmi.twitch.tv/group/user/brentschooley/chatters

		// GetMods()
		public static async Task<List<TwitchViewer>> GetMods()
		{
			var mods = new List<TwitchViewer>();

			var http = new HttpClient();
			var viewersJson = await http.GetStringAsync("https://tmi.twitch.tv/group/user/brentschooley/chatters");

			var json = JObject.Parse(viewersJson);
			var modsJson = json["chatters"]["moderators"];

			foreach (var mod in modsJson)
			{
				var twitchViewer = new TwitchViewer() { Name = mod.ToString(), IsMod = true };
				mods.Add(twitchViewer);
			}

			return mods;
		}

		public static async Task<List<TwitchViewer>> GetViewers()
		{
			var viewers = new List<TwitchViewer>();

			var http = new HttpClient();
			var viewersJson = await http.GetStringAsync("https://tmi.twitch.tv/group/user/brentschooley/chatters");

			var jsonObject = JObject.Parse(viewersJson);
			var twitchViewersArray = jsonObject["chatters"]["viewers"];

			foreach (var viewer in twitchViewersArray)
			{
				var twitchViewer = new TwitchViewer() { Name = viewer.ToString(), IsMod = false };
				viewers.Add(twitchViewer);
			}

			return viewers;
		}

		public static async Task<long?> GetViewerCount()
		{
			var twixel = new Twixel ("iy9ihx1dt50wo7jsb5p8tv5kvpd1yvq", "http://localhost");
			var stream = await twixel.RetrieveStream ("brentschooley");
			return stream.viewers;
		}
	}
}

