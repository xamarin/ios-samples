using UIKit;

namespace CloudKitAtlas
{
	public class CodeSampleGroup
	{
		public string Title { get; }
		public UIImage Icon { get; }
		public CodeSample [] CodeSamples { get; }

		public CodeSampleGroup (string title, UIImage icon, CodeSample [] codeSamples)
		{

			Title = title;
			Icon = icon;
			CodeSamples = codeSamples;

		}
	}
}