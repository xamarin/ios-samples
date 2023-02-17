using Foundation;
using UIKit;

namespace KannadaKeyboardCompanion {
	public partial class WebElement : UIViewController {
		public string HtmlFile { get; set; }

		public WebElement () : base ("WebElement", null)
		{
		}

		public void SetPageTitle (string title)
		{
			Title = title;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			var path = NSBundle.MainBundle.PathForResource (HtmlFile, "html");
			using (var data = NSData.FromFile (path)) {
				var html = new NSString (data, NSStringEncoding.UTF8);
				InstructionWebView.LoadHtmlString (html.ToString (), NSBundle.MainBundle.BundleUrl);
			}
		}
	}
}

