using System.Collections;
using System.Collections.Generic;

using Foundation;

namespace MessagesExtension {
	public class IceCreamHistory : IEnumerable<IceCream> {
		const string UserDefaultsKey = "iceCreams";

		List<IceCream> IceCreams { get; set; }

		public int Count {
			get {
				return IceCreams.Count;
			}
		}

		public IceCream this [int i] {
			get {
				return IceCreams [i];
			}
			set {
				IceCreams.Insert (i, value);
			}
		}

		IceCreamHistory (IceCream[] iceCreams)
		{
			IceCreams = new List<IceCream> (iceCreams);
		}

		public static IceCreamHistory Load ()
		{
			var iceCreams = new List<IceCream> ();
			var defaults = NSUserDefaults.StandardUserDefaults;

			var serializedList = defaults.ValueForKey ((NSString)UserDefaultsKey) as NSString;

			var savedIceCreams = new string [0];
			if (!string.IsNullOrEmpty (serializedList))
				savedIceCreams = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]> (serializedList);

			foreach (var savedIceCream in savedIceCreams) {
				var url = new NSUrl (savedIceCream);
				var components = NSUrlComponents.FromUrl (url, false);
				iceCreams.Add (new IceCream (components.QueryItems));
			}

			if (iceCreams.Count == 0) {
				iceCreams.Add (new IceCream (BaseType.base01, ScoopsType.scoops05, ToppingType.topping09));
				iceCreams.Add (new IceCream (BaseType.base03, ScoopsType.scoops07, ToppingType.topping01));
				iceCreams.Add (new IceCream (BaseType.base04, ScoopsType.scoops08, ToppingType.topping07));
				iceCreams.Add (new IceCream (BaseType.base02, ScoopsType.scoops03, ToppingType.topping10));
				iceCreams.Add (new IceCream (BaseType.base01, ScoopsType.scoops01, ToppingType.topping05));

				var historyToSave = new IceCreamHistory (iceCreams.ToArray ());
				historyToSave.Save ();
			}

			return new IceCreamHistory (iceCreams.ToArray());
		}

		public void Append (IceCream item)
		{
			IceCreams.Add (item);
		}

		public void Save ()
		{
			var iceCreamsToSave = IceCreams;
			var iceCreamURLStrings = new List<string> ();

			foreach (var iceCream in iceCreamsToSave) {
				var components = new NSUrlComponents {
					QueryItems = iceCream.QueryItems
				};

				iceCreamURLStrings.Add (components.Url.AbsoluteString);
			}

			var defaults = NSUserDefaults.StandardUserDefaults;
			var serializedList = Newtonsoft.Json.JsonConvert.SerializeObject (iceCreamURLStrings);
			defaults.SetString (serializedList, UserDefaultsKey);
		}

		public IEnumerator<IceCream> GetEnumerator ()
		{
			return IceCreams.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}
}

