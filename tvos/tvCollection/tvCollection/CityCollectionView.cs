using System;
using Foundation;
using UIKit;

namespace tvCollection
{
	public partial class CityCollectionView : UICollectionView
	{
		#region Application Access
		public static AppDelegate App {
			get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
		}
		#endregion

		#region Computed Properties
		public CityViewDatasource Source {
			get { return DataSource as CityViewDatasource;}
		}

		public CityCollectionViewController ParentController { get; set;}
		#endregion

		#region Constructors
		public CityCollectionView (IntPtr handle) : base (handle)
		{
			// Initialize
			RegisterClassForCell (typeof(CityCollectionViewCell), CityViewDatasource.CardCellId);
			DataSource = new CityViewDatasource (this);
			Delegate = new CityViewDelegate ();
		}
		#endregion

		#region Override Methods
		public override nint NumberOfSections ()
		{
			return 1;
		}

		public override void DidUpdateFocus (UIFocusUpdateContext context, UIFocusAnimationCoordinator coordinator)
		{
			var previousItem = context.PreviouslyFocusedView as CityCollectionViewCell;
			if (previousItem != null) {
				Animate (0.2, () => {
					previousItem.CityTitle.Alpha = 0.0f;
				});
			}

			var nextItem = context.NextFocusedView as CityCollectionViewCell;
			if (nextItem != null) {
				Animate (0.2, () => {
					nextItem.CityTitle.Alpha = 1.0f;
				});
			}
		}
		#endregion
	}
}
