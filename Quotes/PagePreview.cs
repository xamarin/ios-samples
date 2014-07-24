using System;
using CoreGraphics;

using UIKit;
using CoreFoundation;
using Foundation;
using CoreText;


namespace Quotes
{
	public partial class PagePreview : UICollectionViewCell
	{
		Page cellPage;

		[Export ("initWithCoder:")]
		public PagePreview (NSCoder c) : base (c)
		{
		}

		public PagePreview ()
		{
		}
		
		public Page Page {
			get {
				return cellPage;
			}
			set {
				cellPage = value;

				var textShadow = new NSShadow () {
					ShadowOffset = new CGSize (3, 3),
					ShadowBlurRadius = 2
				};

				var attrs = new NSMutableDictionary () {
					{ UIStringAttributeKey.Shadow, textShadow },
					{ UIStringAttributeKey.ForegroundColor, UIColor.Red },
				};

				var attributedTitle = new NSMutableAttributedString (Page.Title, attrs);
				var bold = UIFont.BoldSystemFontOfSize (17);
				var dashPos = attributedTitle.Value.IndexOf ("-");
				var rangeOfPlayName = new NSRange (dashPos + 1, attributedTitle.Value.Length - dashPos - 1);

				textLabel.TextAlignment = UITextAlignment.Center;
				textLabel.LineBreakMode = UILineBreakMode.WordWrap;
				textLabel.Lines = 0;

				attributedTitle.AddAttribute (UIStringAttributeKey.Font, bold, rangeOfPlayName);
				textLabel.AttributedText = attributedTitle;

				pageImageView.Image = new PageView (pageImageView.Frame).RenderPagePreview (Page, pageImageView.Frame.Size);
			}
		}

		public override bool Highlighted {
			get {
				return base.Highlighted;
			}
			set {
				base.Highlighted = value;
				
				if (value) {
					textLabel.Layer.CornerRadius = 7.5f;
					textLabel.BackgroundColor = UIColor.FromHSBA (0.6f, 0.6f, 0.7f, 1);
				} else {
					textLabel.Layer.CornerRadius = 0.0f;
					textLabel.BackgroundColor = UIColor.Clear;
				}
			}
		}
		
		public void DoSomeWorkThatTakesLong ()
		{
			// we don't have anything to do. Simulate a long-running task by calling sleep. 
			// That is okay here because we are in a demo and not running on the main thread.
			NSThread.SleepFor (2.0);
		}
	}
}

