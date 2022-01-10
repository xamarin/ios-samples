/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Sample news feed controller object.
*/

using System;
using Foundation;

namespace Conference_Diffable.CompositionalLayout.Controllers {
	public class ConferenceNewsController {
		public class NewsFeedItem : NSObject {
			public string Id { get; private set; }
			public string Title { get; set; }
			public DateTime Date { get; set; }
			public string Body { get; set; }

			public NewsFeedItem () : this (null, new DateTime (), null) { }

			public NewsFeedItem (string title, DateTime date, string body)
			{
				Id = new NSUuid ().ToString ();
				Title = title;
				Date = date;
				Body = body;
			}

			public override int GetHashCode () => HashCode.Combine (base.GetHashCode (), Id);
		}

		public NewsFeedItem [] News { get; private set; }

		public ConferenceNewsController () => GenerateNews ();

		void GenerateNews () => News = new [] {
				new NewsFeedItem ("Conference 2019 Registration Now Open", new DateTime (2109, 3, 14), "\"" +
					"Register by Wednesday, March 20, 2019 at 5:00PM PSD for your chance to join us and thousands\n" +
					"of coders, creators, and crazy ones at this year's Conference 19 in San Jose, June 3-7.\""),
				new NewsFeedItem ("Apply for a Conference19 Scholarship", new DateTime (2109, 3, 14), "\"" +
					"Conference Scholarships reward talented studens and STEM orgination members with the opportunity\n" +
					"to attend this year's conference. Apply by Sunday, March 24, 2019 at 5:00PM PDT\""),
				new NewsFeedItem ("Conference18 Video Subtitles Now in More Languages", new DateTime (2109, 8, 8), "\"" +
					"All of this year's session videos are now available with Japanese and Simplified Chinese subtitles.\n" +
					"Watch in the Videos tab or on the Apple Developer website.\""),
				new NewsFeedItem ("Lunchtime Inspiration", new DateTime (2109, 6, 8), "\"" +
					"All of this year's session videos are now available with Japanese and Simplified Chinease subtitles.\n" +
					"Watch in the Videos tab or on the Apple Developer website.\""),
				new NewsFeedItem ("Close Your Rings Challenge", new DateTime (2109, 6, 8), "\"" +
					"Congratulations to all Close Your Rings Challenge participants for staying active\n" +
					"throughout the week! Everyone who participated in the challenge can pick up a\n" +
					"special reward pin outside on the Plaza until 5:00 p.m.\"")
			};
	}
}
