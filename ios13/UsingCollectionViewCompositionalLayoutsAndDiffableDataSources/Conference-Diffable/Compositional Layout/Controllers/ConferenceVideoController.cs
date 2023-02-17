/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Controller object that manages the videos and video collection for the sample app
*/

using System;
using Foundation;

namespace Conference_Diffable.CompositionalLayout.Controllers {
	public class ConferenceVideoController {
		public class Video : NSObject {
			public string Id { get; private set; }
			public string Title { get; set; }
			public string Category { get; set; }

			public Video () : this (null, null) { }

			public Video (string title, string category)
			{
				Id = new NSUuid ().ToString ();
				Title = title;
				Category = category;
			}

			public override int GetHashCode () => HashCode.Combine (base.GetHashCode (), Id);
		}

		public class VideoCollection : NSObject {
			public string Id { get; private set; }
			public string Title { get; set; }
			public Video [] Videos { get; set; }

			public VideoCollection () : this (null, null) { }

			public VideoCollection (string title, Video [] videos)
			{
				Id = new NSUuid ().ToString ();
				Title = title;
				Videos = videos;
			}

			public override int GetHashCode () => HashCode.Combine (base.GetHashCode (), Id);
		}

		public VideoCollection [] Collections { get; private set; }

		public ConferenceVideoController () => GenerateCollections ();

		void GenerateCollections () => Collections = new [] {
				new VideoCollection ("The New iPad Pro", new [] {
					new Video ("Bringing Your Apps to the New iPad Pro", "Tech Talks"),
					new Video ("Designing for iPad Pro and Apple Pencil", "Tech Talks")
				}),
				new VideoCollection ("iPhone and Apple Watch", new [] {
					new Video ("Building Apps for iPhone XS, iPhone XS Max, and iPhone XR", "Tech Talks"),
					new Video ("Designing for Apple Watch Series 4", "Tech Talks"),
					new Video ("Developing Complications for Apple Watch Series 4", "Tech Talks"),
					new Video ("What's New in Core NFC", "Tech Talks")
				}),
				new VideoCollection ("App Store Connect", new [] {
					new Video ("App Store Connect Basics", "App Store Connect"),
					new Video ("App Analytics Retention", "App Store Connect"),
					new Video ("App Analytics Metrics", "App Store Connect"),
					new Video ("App Analytics Overview", "App Store Connect"),
					new Video ("TestFlight", "App Store Connect")
				}),
				new VideoCollection ("Apps on Your Wrist", new [] {
					new Video ("What's new in watchOS", "Conference 2018"),
					new Video ("Updating for Apple Watch Series 3", "Tech Talks"),
					new Video ("Planning a Great Apple Watch Experience", "Conference 2017"),
					new Video ("News Ways to Work with Workouts", "Conference 2018"),
					new Video ("Siri Shortcuts on the Siri Watch Face", "Conference 2018"),
					new Video ("Creating Audio Apps for watchOS", "Conference 2018"),
					new Video ("Designing Notifications", "Conference 2018")
				}),
				new VideoCollection ("Speaking with Siri", new [] {
					new Video ("Introduction to Siri Shortcuts", "Conference 2018"),
					new Video ("Building for Voice with Siri Shortcuts", "Conference 2018"),
					new Video ("What's New in SiriKit", "Conference 2017"),
					new Video ("Making Great SiriKit Experiences", "Conference 2017"),
					new Video ("Increase Usage of You App With Proactive Suggestions", "Conference 2018")
				}),
			};
	}
}
