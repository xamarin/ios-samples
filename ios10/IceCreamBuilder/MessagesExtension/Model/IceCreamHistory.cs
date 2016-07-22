using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Newtonsoft.Json;

namespace MessagesExtension {
	public class IceCreamHistory : IEnumerable<IceCream> {
		const string UserDefaultsKey = "iceCreams";

		readonly List<IceCream> iceCreams;

		public int Count {
			get {
				return iceCreams.Count;
			}
		}

		public IceCream this [int i] {
			get {
				return iceCreams [i];
			}
			set {
				iceCreams.Insert (i, value);
			}
		}

		IceCreamHistory (List<IceCream> iceCreams)
		{
			this.iceCreams = iceCreams;
		}

		public static IceCreamHistory Load ()
		{
			var defaults = NSUserDefaults.StandardUserDefaults;
			var serializedList = defaults.StringForKey (UserDefaultsKey);

			var savedIceCreams = string.IsNullOrWhiteSpace (serializedList)
									   ? new string [0]
									   : JsonConvert.DeserializeObject<string []> (serializedList);

			var iceCreams = savedIceCreams.Select (sic => {
				var components = NSUrlComponents.FromUrl (new NSUrl (sic), false);
				return new IceCream (components.QueryItems);
			}).ToList ();

			if (iceCreams.Count == 0) {
				iceCreams.Add (new IceCream (BaseType.base01, ScoopsType.scoops05, ToppingType.topping09));
				iceCreams.Add (new IceCream (BaseType.base03, ScoopsType.scoops07, ToppingType.topping01));
				iceCreams.Add (new IceCream (BaseType.base04, ScoopsType.scoops08, ToppingType.topping07));
				iceCreams.Add (new IceCream (BaseType.base02, ScoopsType.scoops03, ToppingType.topping10));
				iceCreams.Add (new IceCream (BaseType.base01, ScoopsType.scoops01, ToppingType.topping05));

				var historyToSave = new IceCreamHistory (iceCreams);
				historyToSave.Save ();
			}

			return new IceCreamHistory (iceCreams);
		}

		public void Append (IceCream item)
		{
			iceCreams.Add (item);
		}

		public void Save ()
		{
			var iceCreamUrls = iceCreams.Select (ic => new NSUrlComponents { QueryItems = ic.QueryItems }.Url.AbsoluteString)
			                            .ToArray ();

			var serializedList = JsonConvert.SerializeObject (iceCreamUrls);
			var defaults = NSUserDefaults.StandardUserDefaults;
			defaults.SetString (serializedList, UserDefaultsKey);
		}

		public IEnumerator<IceCream> GetEnumerator ()
		{
			return iceCreams.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}
}

