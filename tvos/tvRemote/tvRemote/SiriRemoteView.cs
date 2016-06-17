using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace tvRemote
{
	public partial class SiriRemoteView : UIView
	{
		#region Private Variables
		private string _buttonPressed = "";
		private string _arrowPressed = "";
		#endregion

		#region Computed Properties
		public string ButtonPressed {
			get { return _buttonPressed; }
			set {
				_buttonPressed = value;
				_arrowPressed = "";
				SetNeedsDisplay ();
			}
		}

		public string ArrowPressed {
			get { return _arrowPressed; }
			set {
				_arrowPressed = value;
				_buttonPressed = "";
				SetNeedsDisplay ();
			}
		}
		#endregion

		#region Constructors
		public SiriRemoteView (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void Draw (CoreGraphics.CGRect rect)
		{
			base.Draw (rect);
			SiriRemote.DrawSiriRemote (ButtonPressed, ArrowPressed);
		}
		#endregion
	}
}
