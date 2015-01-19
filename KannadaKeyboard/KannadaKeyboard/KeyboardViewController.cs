using System;

using Foundation;
using UIKit;
using CoreGraphics;

namespace KannadaKeyboard
{
	public partial class KeyboardViewController : UIInputViewController
	{
		public KeyboardViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var nib = UINib.FromName ("KeyboardView", null); 
			var objects = nib.Instantiate (this, null);
			View = objects [0] as UIView;

			Shift.TouchUpInside += ( sender, e) => {
				UpdateShiftText (!isShiftPressed);
				UpdateKeyboardLayout ();
			};
		}

		void SetKeyTitle (UIView row, string[] titles)
		{
			int i = 0;
			foreach (UIButton item in row) {
				item.SetTitle (titles [i++], UIControlState.Normal);
				Console.WriteLine ("{0} {1}", i, item.Title (UIControlState.Normal));
			}
		}

		void UpdateShiftText (bool ispressed)
		{
			isShiftPressed = ispressed;
			Shift.BackgroundColor = ispressed ? UIColor.GroupTableViewBackgroundColor : UIColor.White;
		}

		partial void ChangeKeyboardPressed (NSObject sender)
		{
			AdvanceToNextInputMode ();
		}

		partial void BackspacePressed (NSObject sender)
		{
			TextDocumentProxy.DeleteBackward ();
		}

		partial void ReturnPressed (NSObject sender)
		{
			TextDocumentProxy.InsertText ("\n");
		}

		partial void SpacePressed (NSObject sender)
		{
			TextDocumentProxy.InsertText (" ");
		}

		partial void KeyPress (NSObject sender)
		{
			var button = sender as UIButton;
			var text = button.Title (UIControlState.Normal);
			Console.WriteLine (text);

			if (!string.IsNullOrEmpty (text)) {
				TextDocumentProxy.InsertText (text);

				if (isShiftPressed) {
					UpdateShiftText (false);
					UpdateKeyboardLayout ();
				}
			}

			Anumate (button);
		}

		static void Anumate (UIButton button)
		{
			UIView.Animate (0.2, () =>  {
				button.Transform = CGAffineTransform.Scale (CGAffineTransform.MakeIdentity (), 2f, 2f);
			}, () =>  {
				button.Transform = CGAffineTransform.Scale (CGAffineTransform.MakeIdentity (), (nfloat)1f, (nfloat)1f);
			});
		}

		void UpdateKeyboardLayout ()
		{
			if (isShiftPressed) {
				SetKeyTitle (Row1, shiftRow1);
				SetKeyTitle (Row2, shiftRow2);
				SetKeyTitle (Row3, shiftRow3);
			} else {
				SetKeyTitle (Row1, normalRow1);
				SetKeyTitle (Row2, normalRow2);
				SetKeyTitle (Row3, normalRow3);
			}

			SetRemainging ();
		}

		void SetRemainging ()
		{
			var title = isShiftPressed ? shiftRow4 : normalRow4;
			for (int i = 1; i <= 9; i++) {
				var button = Row4.Subviews [i] as UIButton;
				button.SetTitle (title [i], UIControlState.Normal);
			}

		}

		bool isShiftPressed;

		string[] shiftRow1 = { "#", "್ರ", "ರ್", "ಜ್ಞ", "ತ್ರ", "ಕ್ಷ", "ಶ್ರ", "(", ")", "ಃ", "ಋ" };
		string[] shiftRow2 = { "ಔ", "ಐ", "ಆ", "ಈ", "ಊ", "ಭ", "ಙ", "ಘ", "ಧ", "ಝ", "ಢ" };
		string[] shiftRow3 = { "ಓ", "ಏ", "ಅ", "ಇ", "ಉ", "ಫ", "ಱ", "ಖ", "ಥ", "ಛ", "ಠ" };
		string[] shiftRow4 = { "", "ಎ", "ಣ", "ಞ", "ೢ", "ಳ", "ಶ", "ಷ", "ಒ", "ೣ", "" };

		string[] normalRow1 = { "೧", "೨", "೩", "೪", "೫", "೬", "೭", "೮", "೯", "೦", "-" };
		string[] normalRow2 =	{ "ೌ", "ೈ", "ಾ", "ೀ", "ೂ", "ಬ", "ಹ", "ಗ", "ದ", "ಜ", "ಡ" };
		string[] normalRow3 = { "ೋ", "ೇ", "್", "ಿ", "ು", "ಪ", "ರ", "ಕ", "ತ", "ಚ", "ಟ" };
		string[] normalRow4 = { "s", "ೆ", "ಂ", "ಮ", "ನ", "ವ", "ಲ", "ಸ", "ಯ", "ೃ", "b" };

	}

}

