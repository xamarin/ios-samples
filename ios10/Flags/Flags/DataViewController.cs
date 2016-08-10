using System;

using Foundation;
using UIKit;

namespace Flags
{
	public partial class DataViewController : UIViewController
	{
		// For more info https://en.wikipedia.org/wiki/Regional_Indicator_Symbol
		const int RegionalIndicatorSymbolLetterA = 0x1F1E6;
		const int BaseOffset = RegionalIndicatorSymbolLetterA - 'A';

		[Outlet("answerLabel")]
		UILabel answerLabel { get; set; }

		[Outlet ("flagLabel")]
		UILabel flagLabel { get; set; }

		[Outlet ("revealButton")]
		UIButton revealButton { get; set; }

		string flag;
		string regionCode;
		string RegionCode {
			get {
				return regionCode;
			}
			set {
				if (regionCode == value)
					return;

				regionCode = value;
				flag = null;
				if (!string.IsNullOrWhiteSpace (regionCode) && regionCode.Length == 2) {
					var regionalLetter1 = BaseOffset + regionCode [0];
					var regionalLetter2 = BaseOffset + regionCode [1];
					flag = char.ConvertFromUtf32 (regionalLetter1) + char.ConvertFromUtf32 (regionalLetter2);
				}
			}
		}

		public DataViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			var rc = RegionCode;
			if (rc == null)
				throw new InvalidProgramException ("No region code has been set");

			// TODO: make sure this works
			answerLabel.Text = NSLocale.CurrentLocale.GetCountryCodeDisplayName (regionCode);
			flagLabel.Text = flag;
		}

		[Action ("revealAnswer:")]
		void revealAnswer (UIButton sender)
		{
			answerLabel.Hidden = false;
			revealButton.Hidden = true;
		}
	}
}
