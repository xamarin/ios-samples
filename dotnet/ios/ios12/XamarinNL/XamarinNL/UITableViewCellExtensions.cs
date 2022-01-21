namespace XamarinNL;

public static class UITableViewCellExtensions
{
	public static void AddTextLabel(this UITableViewCell cell, string text, string secondaryText = "")
	{
		// Beginning in iOS 14, UITableViewCell has to use cell.ContentConfiguration
		// to add text
		if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
		{
			var content = cell.DefaultContentConfiguration;
			content.Text = text;

			if (!string.IsNullOrEmpty(secondaryText))
				content.SecondaryText = secondaryText;

			cell.ContentConfiguration = content;
		}
		else
		{
			cell.TextLabel.Text = text;

			if (!string.IsNullOrEmpty(secondaryText))
				cell.DetailTextLabel.Text = secondaryText;
		}
	}
}
