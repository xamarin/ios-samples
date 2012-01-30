This project shows one way of implementing a bubble-chat rendering
using MonoTouch.Dialog and a custom ChatBubble element.

This is the core of the program:

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

Miguel de Icaza
