//
// This file contains the sample code to use System.Net.HttpClient
// using the HTTP handler selected in the IDE UI (or given to mtouch)
//

using System;
using System.IO;
using System.Threading.Tasks;

namespace HttpClient.Core
{
    public class NetHttpProvider : NetworkProvider
    {
        private readonly bool secure;

        public NetHttpProvider(bool secure)
        {
            this.secure = secure;
        }

        public override async Task<Stream> ExecuteAsync()
        {
            Stream stream = null;
            using (var client = new System.Net.Http.HttpClient())
            {
                stream = await client.GetStreamAsync(secure ? "https://www.xamarin.com" : WisdomUrl);
            }

            return stream;
        }

        public static Type GetHandlerType()
        {
            Type result = null;
            using (var client = new System.Net.Http.HttpClient())
            {
                result = typeof(System.Net.Http.HttpMessageInvoker).GetField("handler", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.GetValue(client).GetType();
            }

            return result;
        }
    }
}