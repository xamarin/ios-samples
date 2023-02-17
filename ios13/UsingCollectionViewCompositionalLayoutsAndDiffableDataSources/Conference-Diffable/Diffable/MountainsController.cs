/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Controller object that manages our Mountain values and allows for searches
*/

using System;
using System.Linq;
using System.Collections.Generic;

using Foundation;

namespace Conference_Diffable.Diffable {
	public class MountainsController {
		public class Mountain : NSObject, IEquatable<Mountain> {
			public string Id { get; private set; }
			public string Name { get; private set; }
			public int Height { get; private set; }

			public Mountain (string name, int height)
			{
				Name = name;
				Height = height;

				Id = new NSUuid ().ToString ();
			}

			public static bool operator == (Mountain left, Mountain right)
			{
				if (ReferenceEquals (left, right))
					return true;

				if (ReferenceEquals (left, null))
					return false;

				if (ReferenceEquals (right, null))
					return false;

				return left.Equals (right);
			}

			public static bool operator != (Mountain left, Mountain right) => !(left == right);
			public override bool Equals (object obj) => this == (Mountain) obj;
			public bool Equals (Mountain other) => Id == other.Id;
			public override int GetHashCode () => HashCode.Combine (base.GetHashCode (), Id);

			public bool Contains (string filter)
			{
				if (string.IsNullOrWhiteSpace (filter)) return true;

				var lowercasedFilter = filter.ToLower ();
				return Name.ToLower ().Contains (lowercasedFilter);
			}
		}

		Mountain [] mountains;

		public MountainsController () => mountains = GenerateMountains ();

		public Mountain [] FilterMountains (string filter = null, int? limit = null)
		{
			var filtered = mountains.Where (m => m.Contains (filter)).ToArray ();

			if (limit != null && limit > 0)
				filtered = filtered.Take (limit.Value).ToArray ();

			return filtered;
		}

		Mountain [] GenerateMountains ()
		{
			var components = MountainsRawData.Mountains;
			var mountains = new List<Mountain> ();

			foreach (var component in components) {
				var mountainComponents = component.Split (',');
				var name = mountainComponents [0];
				var height = int.Parse (mountainComponents [1]);
				mountains.Add (new Mountain (name, height));
			}

			return mountains.ToArray ();
		}
	}
}
