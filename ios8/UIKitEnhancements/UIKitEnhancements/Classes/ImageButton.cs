using System;
using CoreGraphics;
using Foundation;
using UIKit;
using CoreGraphics;
using System.ComponentModel;

namespace UIKitEnhancements
{
	/// <summary>
	/// Creates a "touchable" image view that can be used as a button.
	/// </summary>
	public class ImageButton : UIImageView
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="UIKitEnhancements.ImageButton"/> class.
		/// </summary>
		public ImageButton () : base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UIKitEnhancements.ImageButton"/> class.
		/// </summary>
		/// <param name="coder">Coder.</param>
		public ImageButton (NSCoder coder) : base(coder)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UIKitEnhancements.ImageButton"/> class.
		/// </summary>
		/// <param name="flag">Flag.</param>
		public ImageButton (NSObjectFlag flag) : base(flag)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UIKitEnhancements.ImageButton"/> class.
		/// </summary>
		/// <param name="bounds">Bounds.</param>
		public ImageButton (CGRect bounds) : base(bounds)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UIKitEnhancements.ImageButton"/> class.
		/// </summary>
		/// <param name="ptr">Ptr.</param>
		public ImageButton (IntPtr ptr) : base(ptr)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UIKitEnhancements.ImageButton"/> class.
		/// </summary>
		/// <param name="image">Image.</param>
		public ImageButton(UIImage image): base(image){
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Toucheses the began.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="evt">Evt.</param>
		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			//Inform caller of event
			RaiseTouched ();

			//Pass call to base object
			base.TouchesBegan (touches, evt);
		}

		#endregion 

		#region Events
		/// <summary>
		/// Button touched delegate.
		/// </summary>
		public delegate void ButtonTouchedDelegate (ImageButton button);
		public event ButtonTouchedDelegate Touched;

		/// <summary>
		/// Raises the touched event
		/// </summary>
		private void RaiseTouched(){
			if (this.Touched != null)
				this.Touched (this);
		}
		#endregion 
	}
}

