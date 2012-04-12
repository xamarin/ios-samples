BubbleCell
==========

This project shows one way of implementing a bubble-chat rendering
similar to the iPhone Messages application using MonoTouch.Dialog and
a custom ChatBubble element.    

This also shows how to setup a Login screen that transitions to the
chat.  On startup, it will request a login and password (both are
"Root").

The ChatViewController renders the discussion as well as a styled
entry to get messages from the user and resizes the entry as needed (up to a point). 

This is how the conversation is added:

	var root = new RootElement ("Chat Sample") {
		new Section () {
			new ChatBubble (true, "This is the text on the left, what I find fascinating about this is how many lines can fit!"),
			new ChatBubble (false, "This is some text on the right"),
			new ChatBubble (true, "Wow, you are very intense!"),
			new ChatBubble (false, "oops"),
			new ChatBubble (true, "yes"),
		}
	};
	var chat = new DialogViewController (UITableViewStyle.Plain, root);
	chat.TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

The ChatBubble hardcodes two images for left and right bubbles which
are stretched using iOS UIImage support for stretching images.  The
ChatBubble does not really support configuring fonts or bubble colors,
you will need to modify the source accordingly.  Luckily the entire
bubble code support is less than 100 lines of code.

The bubble images are from Cedric Vandendriessche
(http://about.me/skytrix) and are posted on his web site:

http://www.freshcreations.be/files/Bubbles.zip

The Entry images as well as the foundation for the ChatViewController
are from the AcaniChat sample:

https://github.com/acani/AcaniChat


Miguel de Icaza
