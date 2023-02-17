using System.IO;
using System.Threading.Tasks;
using UIKit;

namespace HttpClient.Core {
	public abstract class NetworkProvider {
		// URL where we fetch the wisdom from
		public const string WisdomUrl = "http://httpbin.org/ip";

		protected void Busy ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
		}

		protected void Done ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

		public abstract Task<Stream> ExecuteAsync ();
	}
}
