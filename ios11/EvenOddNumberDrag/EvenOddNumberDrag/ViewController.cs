using System;
using UIKit;
using Foundation;

namespace EvenOddNumberDrag {
	public partial class ViewController : UIViewController, IUIDragInteractionDelegate, IUIDropInteractionDelegate {
		#region Fields

		Random random;
		UIDropInteraction evenDropInteraction;
		UIDropInteraction oddDropInteraction;

		#endregion

		#region Constructors

		protected ViewController (IntPtr handle) : base (handle) { }

		#endregion

		#region UIViewController overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			SetupDragAndDrop ();

			random = new Random ();
			GenerateNumber ();
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		#endregion

		#region Methods

		protected void SetupDragAndDrop ()
		{
			NumberLabel.UserInteractionEnabled = true;
			EvenNumbersLabel.UserInteractionEnabled = true;
			OddNumbersLabel.UserInteractionEnabled = true;

			var numberDragInteraction = new UIDragInteraction (this);
			NumberLabel.AddInteraction (numberDragInteraction);

			// On iPad, this defaults to true. On iPhone, this defaults to 
			// false. Since this app should work on the iPhone, enable the the 
			// drag interaction.
			numberDragInteraction.Enabled = true;

			evenDropInteraction = new UIDropInteraction (this);
			EvenNumbersLabel.AddInteraction (evenDropInteraction);
			oddDropInteraction = new UIDropInteraction (this);
			OddNumbersLabel.AddInteraction (oddDropInteraction);
		}

		protected void GenerateNumber ()
		{
			NumberLabel.Text = random.Next (500).ToString ();
		}

		#endregion

		#region IUIDragInteractionDelegate

		public UIDragItem [] GetItemsForBeginningSession (UIDragInteraction interaction, IUIDragSession session)
		{
			bool isEven = Convert.ToInt16 (NumberLabel.Text) % 2 == 0;
			var provider = new NSItemProvider (new NSString (NumberLabel.Text));
			var item = new UIDragItem (provider) {
				LocalObject = new NSNumber (isEven)
			};
			return new UIDragItem [] { item };
		}

		#endregion

		#region IUIDropInteractionDelegate

		[Export ("dropInteraction:sessionDidUpdate:")]
		public UIDropProposal SessionDidUpdate (UIDropInteraction interaction, IUIDropSession session)
		{
			UIDropProposal proposal;
			var isEven = (session.Items [0].LocalObject as NSNumber).BoolValue;
			if (interaction == oddDropInteraction && !isEven) {
				proposal = new UIDropProposal (UIDropOperation.Copy);
			} else if (interaction == evenDropInteraction && isEven) {
				proposal = new UIDropProposal (UIDropOperation.Copy);
			} else {
				proposal = new UIDropProposal (UIDropOperation.Forbidden);
			}
			return proposal;
		}

		[Export ("dropInteraction:performDrop:")]
		public void PerformDrop (UIDropInteraction interaction, IUIDropSession session)
		{
			var label = interaction == oddDropInteraction ? OddNumbersLabel : EvenNumbersLabel;
			session.LoadObjects<NSString> (strings => {
				if (String.IsNullOrEmpty (label.Text)) {
					label.Text = strings [0];
				} else {
					label.Text = $"{strings [0]}, {label.Text}";
				}
			});
			GenerateNumber ();
		}
		#endregion
	}
}
