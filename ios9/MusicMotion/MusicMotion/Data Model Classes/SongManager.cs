using System;
using System.Collections.Generic;

using UIKit;

namespace MusicMotion {
	public class SongManager {

		string currentContext = MotionContext.LowIntensity;

		public event EventHandler DidUpdateSongQueue;
		public event EventHandler DidEncounterAuthorizationError;

		public List<Song> SongQueue { get; private set; }

		public string ContextDescription =>
			string.Format ("{0} {1}", currentContext, " Music");

		public SongManager ()
		{
			QueueLowIntensitySongs ();
		}

		void QueueLowIntensitySongs ()
		{
			currentContext = MotionContext.LowIntensity;
			SongQueue = new List<Song> {
				new Song {
					Artist = "Haunting Irish female singer",
					Title = "Something by a haunting Irish female singer",
					AlbumImage = UIImage.FromBundle ("Iceland_016")
				},
				new Song {
					Artist = "Some 70's British rock band",
					Title = "That Champion Song",
					AlbumImage = UIImage.FromBundle ("Italy_008")
				},
				new Song {
					Artist = "An upbeat electronic artist",
					Title = "Another song that validates me",
					AlbumImage = UIImage.FromBundle ("Iceland_005")
				}
			};
		}

		void QueueMediumIntensitySongs ()
		{
			currentContext = MotionContext.MediumIntensity;
			SongQueue = new List<Song> {
				new Song {
					Artist = "Hippie Artist",
					Title = "New age music",
					AlbumImage = UIImage.FromBundle ("Italy_017")
				},
				new Song {
					Artist = "Another Hippie Artist",
					Title = "More new age music",
					AlbumImage = UIImage.FromBundle ("Iceland_017")
				},
				new Song {
					Artist = "Electronic Artist",
					Title = "Tune from that fire chariots movie",
					AlbumImage = UIImage.FromBundle ("Iceland_006")
				}
			};
		}

		void QueueHighIntensitySongs ()
		{
			currentContext = MotionContext.HighIntensity;
			SongQueue = new List<Song> {
				new Song {
					Artist = "That self-serious goth band",
					Title = "Song that begins slowly and builds",
					AlbumImage = UIImage.FromBundle ("Iceland_001")
				},
				new Song {
					Artist = "A 90's rock band",
					Title = "Catchy 120 beats-per-min rock song",
					AlbumImage = UIImage.FromBundle ("Iceland_018")
				},
				new Song {
					Artist = "A roller disco band",
					Title = "Uptempo disco track",
					AlbumImage = UIImage.FromBundle ("Italy_019")
				}
			};
		}

		void QueueDrivingSongs ()
		{
			currentContext = MotionContext.Driving;
			SongQueue = new List<Song> {
				new Song {
					Artist = "Burning Rubber",
					Title = "Smells Great",
					AlbumImage = UIImage.FromBundle ("Lola_006")
				},
				new Song {
					Artist = "Sunny's Podcast",
					Title = "In May",
					AlbumImage = UIImage.FromBundle ("Lola_009")
				},
				new Song {
					Artist = "Drive Time",
					Title = "So Much Traffic",
					AlbumImage = UIImage.FromBundle ("Lola_011")
				}
			};
		}

		public void LowIntensityContextStarted (object sender, EventArgs e)
		{
			QueueLowIntensitySongs ();
			DidUpdateSongQueue?.Invoke (this, null);
		}

		public void MediumIntensityContextStarted (object sender, EventArgs e)
		{
			QueueMediumIntensitySongs ();
			DidUpdateSongQueue?.Invoke (this, null);
		}

		public void HighIntensityContextStarted (object sender, EventArgs e)
		{
			QueueHighIntensitySongs ();
			DidUpdateSongQueue?.Invoke (this, null);
		}

		public void DrivingContextStarted (object sender, EventArgs e)
		{
			QueueDrivingSongs ();
			DidUpdateSongQueue?.Invoke (this, null);
		}

		public void HandleEncounterAuthorizationError (object sender, EventArgs e)
		{
			DidEncounterAuthorizationError?.Invoke (this, null);
		}
	}
}

