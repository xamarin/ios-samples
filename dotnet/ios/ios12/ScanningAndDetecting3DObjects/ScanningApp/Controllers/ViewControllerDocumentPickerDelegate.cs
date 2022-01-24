namespace ScanningAndDetecting3DObjects;

internal class ViewControllerDocumentPickerDelegate : UIDocumentPickerDelegate
{
	Action<NSUrl> handler;

	internal ViewControllerDocumentPickerDelegate (Action<NSUrl> handler)
	{
		this.handler = handler;
	}

	public override void DidPickDocument (UIDocumentPickerViewController controller, NSUrl url)
	{
		handler (url);
	}
}
