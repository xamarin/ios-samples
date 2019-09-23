/*
See LICENSE folder for this sample’s licensing information.

Abstract:
`InsertionSortArray` provides a self sorting array class
*/

using System;
using System.Collections;
using System.IO;
using System.Linq;
using Foundation;
using UIKit;

namespace Conference_Diffable {
	public class InsertionSortArray : NSObject {
		public class SortNode : NSObject, IEquatable<SortNode> {
			public string Id { get; private set; }
			public int Value { get; private set; }
			public UIColor Color { get; private set; }

			public SortNode (int value, int maxValue)
			{
				Value = value;
				var hue = value / (nfloat)maxValue;
				Color = UIColor.FromHSBA (hue, 1, 1, 1);

				Id = new NSUuid ().ToString ();
			}

			public static bool operator == (SortNode left, SortNode right)
			{
				if (ReferenceEquals (left, right))
					return true;

				if (ReferenceEquals (left, null))
					return false;

				if (ReferenceEquals (right, null))
					return false;

				return left.Equals (right);
			}

			public static bool operator != (SortNode left, SortNode right) => !(left == right);
			public override bool Equals (object obj) => this == (SortNode)obj;
			public bool Equals (SortNode other) => Id == other.Id;
			public override int GetHashCode () => HashCode.Combine (base.GetHashCode (), Id);
		}

		int currentIndex = 1;

		public string Id { get; private set; }
		public SortNode [] Nodes { get; set; }
		public bool Sorted { get; set; }

		public InsertionSortArray (int count)
		{
			Nodes = Enumerable.Range (0, count).Select (i =>  new SortNode (i, count)).ToArray ();
			SortNodes ();
			Id = new NSUuid ().ToString ();
		}

		public void SortNext () => PerformNextSortStep ();

		void SortNodes ()
		{
			var nodesCount = Nodes.Length;
			var random = new Random (DateTime.Now.Millisecond);
			for (int i = 0; i < nodesCount; i++) {
				var randomIndex = random.Next (0, nodesCount);
				var node = Nodes [i];
				Nodes [i] = Nodes [randomIndex];
				Nodes [randomIndex] = node;
			}
		}

		void PerformNextSortStep ()
		{
			if (Sorted) return;

			if (Nodes.Length == 1) {
				Sorted = true;
				return;
			}

			var currentNode = Nodes [currentIndex];
			var index = currentIndex - 1;

			while (index >= 0 && currentNode.Value < Nodes [index].Value) {
				var tmp = Nodes [index];
				Nodes [index] = currentNode;
				Nodes [index + 1] = tmp;
				index -= 1;
			}

			currentIndex += 1;

			if (currentIndex >= Nodes.Length)
				Sorted = true;
		}

		public static bool operator == (InsertionSortArray left, InsertionSortArray right)
		{
			if (ReferenceEquals (left, right))
				return true;

			if (ReferenceEquals (left, null))
				return false;

			if (ReferenceEquals (right, null))
				return false;

			return left.Equals (right);
		}

		public static bool operator != (InsertionSortArray left, InsertionSortArray right) => !(left == right);
		public override bool Equals (object obj) => this == (InsertionSortArray)obj;
		public bool Equals (InsertionSortArray other) => Id == other.Id;
		public override int GetHashCode () => HashCode.Combine (base.GetHashCode (), Id);
	}
}
