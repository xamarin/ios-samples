using System;
using UIKit;

namespace PrivacyPrompts
{
	/// <summary>
	/// Every view controller must have at least following controlls
	/// </summary>
	public interface IPrivacyViewController
	{
		UILabel TitleLabel { get; }

		UILabel AccessStatus { get; }

		UIButton RequestAccessButton { get; }

		// Dependency Injection via property
		IPrivacyManager PrivacyManager { get; set; }
	}
}

