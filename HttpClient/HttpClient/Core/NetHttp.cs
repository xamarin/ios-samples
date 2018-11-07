//
// This file contains the sample code to use System.Net.HttpClient
// using the HTTP handler selected in the IDE UI (or given to mtouch)
//

using System.IO;
using System.Threading.Tasks;

namespace HttpClient.Core
{
    public class NetHttp : NetworkProvider
    {
        private readonly bool secure;

        public NetHttp(bool secure)
        {
            this.secure = secure;
        }

        public override async Task<Stream> ExecuteAsync()
        {
            var client = new System.Net.Http.HttpClient();
            //ad.HandlerType = typeof(HttpMessageInvoker).GetField("handler", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue (client).GetType ();
            return await client.GetStreamAsync(secure ? "https://www.xamarin.com" : WisdomUrl);
        }
    }
}