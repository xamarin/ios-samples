using System;
using Foundation;

namespace TableSearch
{
	public class Product : NSObject
	{
		public static readonly string DeviceTypeTitle = "Device type title";
		public static readonly string DesktopTypeTitle = "Desktop type title";
		public static readonly string PortableTypeTitle = "Portable type title";

		public string Title { get; private set; }

		// TODO: how we use it?
		public string HardwareType { get; private set; }

		public int YearIntroduced { get; private set; }

		public double IntroPrice { get; private set; }

		public Product (string hardwareType, string title, int yearIntroduced, double price)
		{
			HardwareType = hardwareType;
			YearIntroduced = yearIntroduced;
			IntroPrice = price;
			Title = title;
		}
	}
}

