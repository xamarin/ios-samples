//
// This sample shows how to use the two Http stacks in MonoTouch:
// The System.Net.WebRequest.
// The MonoTouch.Foundation.NSMutableUrlRequest
//

using UIKit;

namespace HttpClientSample
{
	public class Application
	{
		// URL where we fetch the wisdom from
		public const string WisdomUrl = "http://httpbin.org/ip";

		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}

		public static void Busy ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
		}

		public static void Done ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

	}

}
