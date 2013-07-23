//
// This file contains the sample code to use System.Net.WebRequest
// on the iPhone to communicate with HTTP and HTTPS servers
//
// Author:
//   Miguel de Icaza
//

using System;
using System.Net;
using MonoTouch.Foundation;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;

namespace HttpClient
{
	public class DotNet {
		AppDelegate ad;
		
		public DotNet (AppDelegate ad)
		{
			this.ad = ad;
		}
		
		//
		// Asynchronous HTTP request
		//
		public async Task HttpSample ()
		{
			Application.Busy ();

			var request = WebRequest.Create (Application.WisdomUrl);
			var response = await request.GetResponseAsync ();

			Application.Done ();
			try {
				ad.RenderRssStream (response.GetResponseStream ());
			} catch (Exception e){ 
				Console.WriteLine (e);
				// Error
			}
		}

		//
		// Asynchornous HTTPS request
		//
		public async Task HttpSecureSample (CancellationToken token)
		{
//			try {
				var client = new System.Net.Http.HttpClient ();

				var response = await client.GetAsync ("https://gmail.com", HttpCompletionOption.ResponseContentRead, token);

				Application.Done ();

				var stream = await response.Content.ReadAsStreamAsync ();

				ad.RenderStream (stream);
			 
		}

		//
		// For an explanation of this AcceptingPolicy class, see
		// http://mono-project.com/UsingTrustedRootsRespectfully
		//
		// This will not be needed in the future, when MonoTouch 
		// pulls the certificates from the iPhone directly
		//
		class AcceptingPolicy : ICertificatePolicy {
			public bool CheckValidationResult (ServicePoint sp, X509Certificate cert, WebRequest req, int error)
			{
				// Trust everything
				return true;
			}
		}	
	}
}
