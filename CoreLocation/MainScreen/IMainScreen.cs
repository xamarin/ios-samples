using System;
using UIKit;

namespace Example_CoreLocation
{
	/// <summary>
	/// This interface is so we can use a different xib for iPhone and iPad from the same code.
	/// </summary>
	public interface IMainScreen
	{
		UILabel LblAltitude { get; }
		UILabel LblLatitude { get; }
		UILabel LblLongitude { get; }
		UILabel LblCourse { get; }
		UILabel LblMagneticHeading { get; }
		UILabel LblSpeed { get; }
		UILabel LblTrueHeading { get; }
		UILabel LblDistanceToParis { get; }
	}
}
