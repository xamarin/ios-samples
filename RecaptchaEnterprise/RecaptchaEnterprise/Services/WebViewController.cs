using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CoreGraphics;
//using FocusBrands.Framework.Core.Services.Models;
using Foundation;
using UIKit;
using WebKit;

namespace RecaptchaEnterprise.Services
{
    public class WebViewController : WKWebView, IWKScriptMessageHandler
    {
        public WebViewController(CGRect bounds, NSCoder coder) : base(coder)
        {
        }

        public WebViewController(CGRect frame, WKWebViewConfiguration configuration) : base(frame, configuration)
        {
            Console.WriteLine("loading..");
            /*BackgroundColor = UIColor.Clear;
            ScrollView.BackgroundColor = UIColor.Clear;
            Opaque = false;
            Hidden = true;
            Configuration.UserContentController.AddScriptMessageHandler(this, "recaptcha");*/
        }

        [Export("userContentController:didReceiveScriptMessage:")]
        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            Console.WriteLine(message.Body.ToString());

            if (message.Name == "tokenHandler")
            {
                var token = message.Body as NSString;
                // Handle the reCAPTCHA response token

                Console.WriteLine(token);
            }
        }
    }
}

