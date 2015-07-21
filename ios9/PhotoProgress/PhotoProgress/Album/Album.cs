using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;

namespace PhotoProgress {
	public class Album {

		public List<Photo> Photos { get; set; }

		public Album ()
		{
			var urls = NSBundle.MainBundle.GetUrlsForResourcesWithExtension ("jpg", "Photos");

			if (urls == null || urls.Length == 0)
				throw new Exception ("Unable to load photos");

			Photos = urls.Select (url => new Photo (url)).ToList<Photo> ();
		}

		public PhotoProgress ImportPhotos ()
		{
			var progress = new PhotoProgress {
				TotalUnitCount = Photos.Count
			};

			foreach (var photo in Photos) {
				var importProgress = photo.StartImport ();
				progress.AddChild (importProgress, 1);
			}

			return progress;
		}

		public void ResetPhotos ()
		{
			Photos.ForEach (photo => photo.Reset ());
		}
	}
}

