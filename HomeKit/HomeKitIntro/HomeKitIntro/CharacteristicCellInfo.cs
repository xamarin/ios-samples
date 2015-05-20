using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using HomeKit;

namespace HomeKitIntro
{
	public partial class CharacteristicCellInfo : UITableViewCell
	{
		#region Static Properties
		public static readonly NSString Key = new NSString ("CharacteristicCellInfo");
		#endregion

		#region Constructors
		public CharacteristicCellInfo (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Public Methods
		public void DisplayInfo(string title, string value) {

			// Update UI
			Title.Text = title;
			SubTItle.Text = value;

		}
		#endregion
	}
}
