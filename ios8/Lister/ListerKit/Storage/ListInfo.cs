using System;
using System.Threading.Tasks;

using UIKit;
using Foundation;
using CoreFoundation;
using System.IO;

namespace ListerKit
{
	// TODO: add comments
	public class ListInfo : IEquatable<ListInfo>
	{
		DispatchQueue fetchQueue;

		public ListColor? Color { get; set; }

		public NSUrl Url { get; private set; }

		public string Name {
			get {
				string displayName = NSFileManager.DefaultManager.DisplayName (Url.Path);
				return Path.GetFileNameWithoutExtension (displayName);
			}
		}

		public ListInfo(NSUrl url)
		{
			Url = url;
			fetchQueue = new DispatchQueue ("fetch queue");
		}

		void FetchInfoWithCompletionHandler(Action completionHandler)
		{
			if (completionHandler == null)
				throw new ArgumentNullException ("completionHandler is null");

			fetchQueue.DispatchAsync (() => {
				// If the color hasn't been set yet, the info hasn't been fetched.
				if(Color.HasValue) {
					completionHandler();
					return;
				}

				ListUtilities.ReadListAtUrl(Url, (list, error) => {
					fetchQueue.DispatchAsync(()=> {
						Color = list != null ? list.Color : ListColor.Gray;
						completionHandler();
					});
				});
			});
		}

		// look how to override Equals http://msdn.microsoft.com/en-us/library/336aedhh(v=vs.100).aspx
		// this will be called during search within arrays and lists
		public override bool Equals (object obj)
		{
			if (obj == null || GetType () != obj.GetType ())
				return false;

			ListInfo listInfo = (ListInfo)obj;
			return (Url == null && listInfo.Url == null) || Url.Equals(listInfo.Url);
		}

		public bool Equals (ListInfo other)
		{
			if (other == null || GetType () != other.GetType ())
				return false;

			return (Url == null && other.Url == null) || Url.Equals(other.Url);
		}

		public override int GetHashCode ()
		{
			if (Url == null)
				return 0;
			else
				return Url.GetHashCode ();
		}
	}
}