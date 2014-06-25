
#define USE_AUTOLAYOUT
#define USE_FULLSCREEN_LAYOUT
#define OVERRIDE_SUPPORTED_ORIENTATIONS
using System;
using System.Collections.Generic;
using CoreGraphics;
		
using Foundation;
using UIKit;
		
namespace MediaNotes
{

	[Register ("PNContainerView")]
	public class PNContainerView : UIView {
		public PNContainerView (IntPtr ptr) : base (ptr) {
		
		}

		[Export ("requiresConstraintBasedLayout")]
		static bool Requires ()
		{
			return true;
		}
	}

	public partial class PhotoViewController : UIViewController, YYCommentNotifying
	{
		static Dictionary<NSUrl, string> photoMap;
		static NSUrl currentPhotoUrl;
		bool syncIsNeeded;
		YYCommentViewController ycommentView;
		PNDataSourceProtocol datasource;
				
# if USE_AUTOLAYOUT
		NSLayoutConstraint toolbarTopConstraint;
#endif
		public PNDataSourceProtocol Datasource{ get { return datasource; } set { datasource = value; } }

		public PhotoViewController () : base ("PhotoViewController", null)
		{
		}
				
		public PhotoViewController (string nibName, NSBundle bundle) : base (NSObjectFlag.Empty)
		{
			photoMap = new Dictionary<NSUrl, string> (); 
# if USE_FULLSCREEN_LAYOUT
			WantsFullScreenLayout = true;
#endif
		}
				
		public override void DidReceiveMemoryWarning ()
		{
			photoMap.Clear ();
			View = null;
			photoImageView = null;
			toolbar = null;
			syncIsNeeded = true;
			base.DidReceiveMemoryWarning ();
		}
				
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
# if USE_AUTOLAYOUT
			toolbar.TranslatesAutoresizingMaskIntoConstraints = false;
			photoImageView.TranslatesAutoresizingMaskIntoConstraints = false;
					
# endif			// Perform any additional setup after loading the view, typically from a nib.
		}

				
/*# if OVERRIDE_SUPPORTED_ORIENTATIONS
				
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return UIInterfaceOrientationMask.AllButUpsideDown;
		}
	
# endif*/
				
		public override void ViewWillLayoutSubviews ()
		{
			if (syncIsNeeded) {
				Synchronize ();
				syncIsNeeded = false;
			}
		}
				
# if USE_AUTOLAYOUT
		nfloat GetStatusBarHeight (UIApplication app)
		{
			if (app.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || app.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight)
				return app.StatusBarFrame.Size.Width;
			return app.StatusBarFrame.Size.Height;
		}
				
		public override void UpdateViewConstraints ()
		{
			nfloat toolbarVerticalOffset = WantsFullScreenLayout ? GetStatusBarHeight (UIApplication.SharedApplication) : 0;
					
			if (toolbarTopConstraint == null) {
				var tconstraint2 = NSLayoutConstraint.Create (toolbar, NSLayoutAttribute.Top, NSLayoutRelation.Equal,
                                                             View, NSLayoutAttribute.Top, 1.0f, toolbarVerticalOffset);
				toolbarTopConstraint = tconstraint2;
						
				var tconstraint1 = NSLayoutConstraint.Create (toolbar, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
                                                             View, NSLayoutAttribute.Width, 1.0f, 0.0f);
						
				var tconstraint3 = NSLayoutConstraint.Create (toolbar, NSLayoutAttribute.Left, NSLayoutRelation.Equal,
                                                             View, NSLayoutAttribute.Left, 1.0f, 0.0f);
						
				var tconstraint4 = NSLayoutConstraint.Create (toolbar, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                                                             toolbar, NSLayoutAttribute.Height, 0.0f, 44.0f);
						
				var constraint0 = NSLayoutConstraint.Create (photoImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
                                                            photoImageView, NSLayoutAttribute.Height, 1.0f, 0.0f);
						
				var constraint1 = NSLayoutConstraint.Create (photoImageView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal,
                                                            View, NSLayoutAttribute.CenterX, 1.0f, 0.0f);
						
				NSLayoutConstraint constraint2;
						
				if (WantsFullScreenLayout) {
					constraint2 = NSLayoutConstraint.Create (photoImageView, NSLayoutAttribute.Top, NSLayoutRelation.GreaterThanOrEqual, View,
							                                         NSLayoutAttribute.Top, 1.0f, 0.0f);
				} else {
					constraint2 = NSLayoutConstraint.Create (photoImageView, NSLayoutAttribute.Top, NSLayoutRelation.GreaterThanOrEqual, toolbar,
							                                         NSLayoutAttribute.Bottom, 1.0f, 0.0f);
				}
				NSLayoutConstraint constraint3 = NSLayoutConstraint.Create (photoImageView, NSLayoutAttribute.Bottom, NSLayoutRelation.LessThanOrEqual,
						                                                            View, NSLayoutAttribute.Bottom, 1.0f, 0.0f);



				View.AddConstraints (new NSLayoutConstraint[] {tconstraint1, tconstraint2, tconstraint3, tconstraint4, constraint0, constraint1, constraint2, constraint3});

				toolbarTopConstraint.Constant = toolbarVerticalOffset;
						
			}
			base.UpdateViewConstraints ();
		}
# endif
		public void Synchronize ()
		{
			placeHolderActivityView.StopAnimating ();
			placeHolderLabel.Text = null;
			photoImageView.Image = datasource.ImageForCurrentItem ();
			currentPhotoUrl = datasource.UrlForCurrentItem ();
			Console.WriteLine (currentPhotoUrl);
			       
			placeHolderView.RemoveFromSuperview ();
			placeHolderActivityView = null;
			placeHolderView = null;
			placeHolderLabel = null;
		        
		}

		public string AssociatedComment ()
		{
			string comment;
			if (photoMap.TryGetValue (currentPhotoUrl, out comment))
				Console.WriteLine ("Found comment");
			else
				Console.WriteLine ("Not Found");
			//  comment = photoMap[currentPhotoUrl];
			if (comment == null) {
				comment = "A random comment goes here";
				photoMap.Add (currentPhotoUrl, comment);
			}
			comment = photoMap [currentPhotoUrl];
			Console.WriteLine (comment);
			return comment;
		}
		
		public List<UIImage> ItemsForSharing ()
		{
			List<UIImage> AddImage = new List<UIImage> ();
			UIImage image = datasource.ImageForCurrentItem ();
			if (image != null) {
				AddImage.Add (image);
				return AddImage;
			} else {
				return null;
			}
		}
		
		public void AssociatedCommentDidChange (string comment)
		{
			photoMap.Add (currentPhotoUrl, comment);
		}
				
		public void Synchronize (bool initializationSucceeded)
		{
			bool finishedp;
			if (initializationSucceeded == true) {

				placeHolderActivityView.StopAnimating ();
				photoImageView.Image = datasource.ImageForCurrentItem ();
				currentPhotoUrl = datasource.UrlForCurrentItem ();
				var yvc = ParentViewController as YYCommentContainerViewController;
				ycommentView = yvc.YYcommentViewController ();
				ycommentView .AssociatedObjectDidChange (this); 

				UIView.Animate (.25, () => {
						placeHolderActivityView.Alpha = 0.0f;
					},
					() => {
						placeHolderView.RemoveFromSuperview ();
						placeHolderActivityView = null;
						placeHolderView = null;
						placeHolderLabel = null;
					
					});
				}
			else {

				placeHolderActivityView.StopAnimating ();
				placeHolderLabel.Text = "No Photos";
			}
		}
				
		partial void NextPhoto (UIKit.UIBarButtonItem sender)
		{       
			datasource.ProceedToNextItem ();

			if(currentPhotoUrl != null){
				currentPhotoUrl = datasource.UrlForCurrentItem ();

				ycommentView.AssociatedObjectDidChange (this);
				UIView.Animate (.25, () => {
						photoImageView.Alpha = 0.0f;
					},
					() => {
						photoImageView.Image = datasource.ImageForCurrentItem ();
						UIView.Animate (.25, () => {
							photoImageView.Alpha = 1.0F;
						});
					});
			}
		}
				
		partial  void PreviousPhoto (UIKit.UIBarButtonItem sender)
		{
			datasource.ProceedToPreviousItem ();
			if (currentPhotoUrl != null) {
				currentPhotoUrl = datasource.UrlForCurrentItem ();
				ycommentView.AssociatedObjectDidChange (this);
				UIView.Animate (.25, () => {
					photoImageView.Alpha = 0.0f;
				}, () => {
					photoImageView.Image = datasource.ImageForCurrentItem ();
					UIView.Animate (.25, () => {
						photoImageView.Alpha = 1.0f;
					});
				});
			}	      
		}
	}
}
				
					
					
				

		