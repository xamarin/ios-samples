using System;

using UIKit;
using Foundation;

using Common;
using ListerKit;
using System.Threading.Tasks;

namespace Lister
{
	public class ListInfo : IEquatable<ListInfo>
	{
		public ListColor? Color { get; set; }

		public NSUrl Url { get; private set; }

		public string Name { get; set; }

		public bool IsLoaded {
			get {
				return !string.IsNullOrEmpty (Name) && Color.HasValue;
			}
		}

		public ListInfo(NSUrl url)
		{
			Url = url;
			Name = string.Empty;
		}

		public Task FetchInfoAsync()
		{
			var tcs = new TaskCompletionSource<object> ();

			if (IsLoaded)
				tcs.SetResult (null);
			else
				FetchInfo (tcs);

			return tcs.Task;
		}

		async void FetchInfo(TaskCompletionSource<object> tcs)
		{
			ListDocument document = new ListDocument (Url);

			bool success = await document.OpenAsync ();
			if (success) {
				Color = document.List.Color;
				Name = document.LocalizedName;

				tcs.SetResult (null);
				document.Close(null);
			} else {
				tcs.SetException (new InvalidOperationException ("Your attempt to open the document failed."));
			}
		}

		public void CreateAndSaveWithCompletionHandler(UIOperationHandler completionHandler)
		{
			List list = new List ();
			list.Color = Color.Value;

			ListDocument document = new ListDocument (Url);
			document.List = list;

			document.Save (Url, UIDocumentSaveOperation.ForCreating, completionHandler);
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
