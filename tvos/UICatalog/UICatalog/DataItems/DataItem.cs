using System;
using System.Collections.Generic;

using Foundation;

namespace UICatalog {
	public class DataItem {

		static DataItem[] sampleItems;
		public static DataItem[] SampleItems {
			get {
				if (sampleItems == null) {
					var items = new List<DataItem> ();
					foreach (Group groupType in Enum.GetValues (typeof(Group))) {
						int itemCount = 0;

						switch (groupType) {
						case Group.Scenery:
							itemCount = 6;
							break;
						case Group.Iceland:
							itemCount = 8;
							break;
						case Group.Lola:
							itemCount = 4;
							break;
						case Group.Baby:
							itemCount = 8;
							break;
						}

						for (int i = 1; i <= itemCount; i++)
							items.Add (new DataItem (groupType, i));
					}

					sampleItems = items.ToArray ();
				}

				return sampleItems;
			}
		}

		int number;

		public Group Group { get; private set; }

		static NSNumberFormatter numberFormatter;
		static NSNumberFormatter NumberFormatter {
			get {
				numberFormatter = numberFormatter ?? new NSNumberFormatter { NumberStyle = NSNumberFormatterStyle.SpellOut };
				return numberFormatter;
			}
		}

		public string ImageName {
			get {
				return Group == Group.Scenery ? string.Format ("{0} {1}", Group, number) :
					string.Format ("{0} {1}.jpg", Group, number);
			}
		}

		public string LargeImageName {
			get {
				return Group == Group.Scenery ? string.Format ("{0} {1} Large", Group, number) :
					string.Format ("{0} {1} Large.jpg", Group, number);
			}
		}

		public string Title {
			get {
				return string.Format ("{0} {1}", Group, NumberFormatter.StringFromNumber (NSNumber.FromInt32 (number)));
			}
		}

		public string Identifier {
			get {
				return string.Format ("{0}.{1}", Group, number);
			}
		}

		NSUrl displayURL;
		NSUrl DisplayURL {
			get {
				if (displayURL == null) {
					var components = new NSUrlComponents ();
					components.Scheme = "uikitcatalog";
					components.Path = "dataItem";
					components.QueryItems = new [] { new NSUrlQueryItem ("identifier", Identifier) };
					displayURL = components.Url;
				}

				return displayURL;
			}
		}

		public DataItem (Group group, int number)
		{
			Group = group;
			this.number = number;
		}

		public override bool Equals (object obj)
		{
			if (obj == null || GetType () != obj.GetType()) 
				return false;

			var dataItem = (DataItem)obj;
			return Identifier == dataItem.Identifier;
		}

		public override int GetHashCode () 
		{
			int hash = 17;
			hash = hash * 23 + Title.GetHashCode();
			hash = hash * 23 + Identifier.GetHashCode();
			hash = hash * 23 + DisplayURL.GetHashCode();
			return hash;
		}
	}
}

