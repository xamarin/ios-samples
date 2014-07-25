using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Linq;
using System.Json;
using System.Web;
using Foundation;

namespace SearchDemo
{
    public delegate void SynchronizerDelegate (List<SearchItem> results);

    public class Bing
    {
        const string AZURE_KEY = "Enter API key here";

        static SynchronizerDelegate sync;
        
        public Bing (SynchronizerDelegate sync)
        {
            Bing.sync = sync;
        }

        public void Search (string text)
        {
            var t = new Thread (Search);
            t.Start (text);
        }

        void Search (object text)
        {
            string bingSearch = String.Format ("https://api.datamarket.azure.com/Data.ashx/Bing/Search/v1/Web?Query=%27{0}%27&$top=10&$format=Json", text);

            var httpReq = (HttpWebRequest)HttpWebRequest.Create (new Uri (bingSearch));

            httpReq.Credentials = new NetworkCredential (AZURE_KEY, AZURE_KEY);

            try {
                using (HttpWebResponse httpRes = (HttpWebResponse)httpReq.GetResponse ()) {

                    var response = httpRes.GetResponseStream ();
                    var json = (JsonObject)JsonObject.Load (response);
            
                    var results = (from result in (JsonArray)json ["d"] ["results"]
                                let jResult = result as JsonObject 
                                select new SearchItem { Title = jResult["Title"], Url = jResult["Url"] }).ToList ();
               
                    if (sync != null)
                        sync (results);
                }
            } catch (Exception) {
                if (sync != null)
                    sync (null);
            }
        }

    }
}