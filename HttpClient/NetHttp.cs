//
// This file contains the sample code to use System.Net.HttpClient
// on the iPhone to communicate using Apple's CFNetwork API
//

using System;
using System.Threading.Tasks;
using System.Net.Http;

namespace HttpClient
{
	public class NetHttp
	{
		AppDelegate ad;

		public NetHttp (AppDelegate ad)
		{
			this.ad = ad;
		}

		public async Task HttpSample ()
		{
			var client = new System.Net.Http.HttpClient (new CFNetworkHandler ());
			ad.RenderRssStream (await client.GetStreamAsync (Application.WisdomUrl));
		}
	}
}

