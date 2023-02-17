using Foundation;
using MonoTouch.Dialog;

using XMBindingLibrary;

namespace XMBindingLibrarySample {
	public class UtilitiesViewController : DialogViewController {
		XMUtilities utility;

		public UtilitiesViewController ()
			: base (new RootElement ("XMUtilities Binding"), true)
		{
		}

		public override void LoadView ()
		{
			base.LoadView ();

			utility = new XMUtilities ();

			utility.SetCallback (new XMUtilityCallback (message => {
				SetResultElementValue ("Callback: " + message);
			}));

			Root.Add (new []
			{
				new Section("Operands")
				{
					new EntryElement("Name: ", "", ""),
					new EntryElement("Operand One: ", "", ""),
					new EntryElement("Operand Two: ", "", ""),
				},

				new Section("Result")
				{
					new StringElement("")
				},

				new Section("Operations")
				{
					new StringElement("Add", Handle_AddOperation),
					new StringElement("Subtract", Handle_SubtractOperation),
					new StringElement("Multiply", Handle_MultiplyOperation),
					new StringElement("Echo", Handle_EchoOperation),
					new StringElement("Speak", Handle_SpeakOperation),
					new StringElement("Speak Greeting", Handle_SpeakGreetingOperation),
					new StringElement("Hello", Handle_HelloOperation),
					new StringElement("Invoke Callback", Handle_InvokeCallback),
				},

				new Section("More Bindings")
				{
					new StringElement("Go to CustomView!", () =>
					{
						var c = new CustomViewController();
						NavigationController.PushViewController(c, true);
					})
				}
			});
		}

		void Handle_AddOperation ()
		{
			var one = Root [0] [1] as EntryElement;
			var two = Root [0] [2] as EntryElement;

			int.TryParse (one.Value, out var un);
			int.TryParse (two.Value, out var deux);

			SetResultElementValue ($"{utility.Add (un, deux)}");
		}

		void Handle_SubtractOperation ()
		{
			var one = Root [0] [1] as EntryElement;
			var two = Root [0] [2] as EntryElement;

			int.TryParse (one.Value, out var un);
			int.TryParse (two.Value, out var deux);

			SetResultElementValue ($"{utility.Subtract (un, deux)}");
		}

		void Handle_MultiplyOperation ()
		{
			var one = Root [0] [1] as EntryElement;
			var two = Root [0] [2] as EntryElement;

			int.TryParse (one.Value, out var un);
			int.TryParse (two.Value, out var deux);

			SetResultElementValue ($"{utility.Multiply (un, deux)}");
		}

		void Handle_EchoOperation ()
		{
			var nameElement = Root [0] [0] as EntryElement;
			SetResultElementValue (XMUtilities.Echo (nameElement.Value));
		}

		void Handle_SpeakOperation ()
		{
			SetResultElementValue (utility.Speak ());
		}

		void Handle_SpeakGreetingOperation ()
		{
			SetResultElementValue (utility.Speak (XMGreeting.Goodbye));
		}

		void Handle_HelloOperation ()
		{
			var nameElement = Root [0] [0] as EntryElement;
			SetResultElementValue (utility.Hello (nameElement.Value));
		}

		void Handle_InvokeCallback ()
		{
			utility.InvokeCallback ("Callback invoked!");
		}

		void SetResultElementValue (string value)
		{
			NSThread.Current.BeginInvokeOnMainThread (() => {
				if (Root [1] [0] is StringElement e) {
					e.Caption = value;

					TableView.ReloadData ();
				}
			});
		}
	}
}
