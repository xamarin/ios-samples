
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Example_SplitView.Screens.DetailView
{
	public partial class DetailViewScreen : UIViewController
	{
	
		/// <summary>
		/// The text for the item number label. I expose this to show a way 
		/// you can communicate between master and detail view.
		/// </summary>
		public string Text
		{
			get { return lblItemNumber.Text; }
			set { lblItemNumber.Text = value; }
		}

		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public DetailViewScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public DetailViewScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public DetailViewScreen () : base("DetailViewScreen", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		/// <summary>
		/// 
		/// </summary>
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		public void AddContentsButton (UIBarButtonItem button)
		{
			button.Title = "Contents";
			tlbrTop.SetItems (new UIBarButtonItem[] { button }, false );
		}
		
		public void RemoveContentsButton ()
		{
			tlbrTop.SetItems (new UIBarButtonItem[0], false);
		}
		
	}
}

