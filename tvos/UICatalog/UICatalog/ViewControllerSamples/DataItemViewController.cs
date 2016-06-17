using Foundation;
using UIKit;

namespace UICatalog {
	public partial class DataItemViewController : UIViewController {

		public static readonly string StoryboardIdentifier = "DataItemViewController";

		public DataItem DataItem { get; private set; }

		[Export ("initWithCoder:")]
		public DataItemViewController (NSCoder coder): base (coder)
		{
		}

		public override void ViewDidLoad ()
		{
			ImageView.Image = UIImage.FromBundle (DataItem.LargeImageName);
		}

		public void ConfigureWithDataItem (DataItem dataItem) 
		{
			DataItem = dataItem;
		}
	}
}
