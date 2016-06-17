using System;

using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	[Register ("TextInputController")]
	partial class TextInputController
	{
		[Action ("replyWithTextInputController:")]
		partial void ReplyWithTextInputController (NSObject obj);
	}
}

