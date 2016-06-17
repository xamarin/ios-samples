using System;
using Foundation;
using UIKit;
using CoreGraphics;

namespace tvCollection
{
	public partial class CityCollectionViewCell : UICollectionViewCell
	{
		#region Private Variables
		private CityInfo _city;
		#endregion

		#region Computed Properties
		public UIImageView CityView { get ; set; }
		public UILabel CityTitle { get; set; }

		public CityInfo City {
			get { return _city; }
			set {
				_city = value;
				CityView.Image = UIImage.FromFile (City.ImageFilename);
				CityView.Alpha = (City.CanSelect) ? 1.0f : 0.5f;
				CityTitle.Text = City.Title;
			}
		}
		#endregion

		#region Constructors
		public CityCollectionViewCell (IntPtr handle) : base (handle)
		{
			// Initialize
			CityView = new UIImageView(new CGRect(22, 19, 320, 171));
			CityView.AdjustsImageWhenAncestorFocused = true;
			AddSubview (CityView);

			CityTitle = new UILabel (new CGRect (22, 209, 320, 21)) {
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.White,
				Alpha = 0.0f
			};
			AddSubview (CityTitle);
		}
		#endregion
	

	}
}
