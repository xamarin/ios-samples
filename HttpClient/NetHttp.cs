//
// This file contains the sample code to use System.Net.HttpClient
// using the HTTP handler selected in the IDE UI (or given to mtouch)
//

using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Net.Http;

namespace HttpClientSample
{
	public class NetHttp
	{
		AppDelegate ad;

		public NetHttp (AppDelegate ad)
		{
			this.ad = ad;
		}

		public async Task HttpSample (bool secure)
		{
			var client = new HttpClient ();
			ad.HandlerType = typeof(HttpMessageInvoker).GetField("handler", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue (client).GetType ();
			ad.RenderStream (await client.GetStreamAsync (secure ? "https://gmail.com" : Application.WisdomUrl));
		}
	}
}

