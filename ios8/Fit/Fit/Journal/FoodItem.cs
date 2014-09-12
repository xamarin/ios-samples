using System;
using Foundation;

namespace Fit
{
	public class FoodItem : NSObject
	{
		public string Name { get; private set; }

		public double Joules { get; private set; }

		public override string Description {
			get {
				return string.Format ("name {0}, joules {1}", Name, Joules);
			}
		}

		public static FoodItem Create (string name, double joules)
		{
			return new FoodItem {
				Name = name,
				Joules = joules
			};
		}

		public FoodItem ()
		{
		}

		public override bool Equals (object obj)
		{
			if (obj.GetType () == typeof(FoodItem))
				return ((FoodItem)obj).Joules == Joules && ((FoodItem)obj).Name == Name;

			return false;
		}

		public override int GetHashCode ()
		{
			return Name.GetHashCode () ^ Joules.GetHashCode ();
		}
	}
}

