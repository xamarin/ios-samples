using System;
using System.Net;

namespace LazyTableImages {
	public class GzipWebClient : WebClient {
		protected override WebRequest GetWebRequest (Uri address)
		{
			var request = base.GetWebRequest (address);
			if (request is HttpWebRequest)
				((HttpWebRequest) request).AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
			return request;
		}
	}
}

