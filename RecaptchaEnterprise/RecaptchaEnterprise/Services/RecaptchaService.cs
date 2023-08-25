using System;
using System.Threading.Tasks;
//using FocusBrands.Framework.Core.Services.Models;
using Foundation;
using RecaptchaEnterprise.Models;
using UIKit;
using WebKit;
using RecaptchaEnterprise.Interfaces;

namespace FocusBrands.Framework.Presentation.Services.Apple
{
    public class RecaptchService : IRecaptchaService
    {
        private TaskCompletionSource<string> _tcsWebView;

        private TaskCompletionSource<RecaptchaResponse> _authorizationComplete;

        private RecaptchaWebView _reCaptchaWebView;

        public async Task<RecaptchaResponse> InitializeRecaptch()
        {
            _authorizationComplete = new TaskCompletionSource<RecaptchaResponse>();

            var test = Verify("6LcdBMonAAAAAOt4Im1zkQ0Oioc8VDM8zvGtHNyc", "www.google.com");

            var response = test;

            return await _authorizationComplete.Task;
        }

        public Task<string> Verify(string siteKey, string domainUrl)
        {
            _tcsWebView = new TaskCompletionSource<string>();

            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            var webViewConfiguration = new WKWebViewConfiguration();
            _reCaptchaWebView = new RecaptchaWebView(window.Bounds, webViewConfiguration)
            {
                SiteKey = siteKey,
                DomainUrl = domainUrl
            };
            _reCaptchaWebView.ReCaptchaCompleted += RecaptchaWebViewViewControllerOnReCaptchaCompleted;

#if DEBUG
            // Forces the Captcha Challenge to be explicitly displayed
            _reCaptchaWebView.PerformSelector(new ObjCRuntime.Selector("setCustomUserAgent:"), NSThread.MainThread, new NSString("Googlebot/2.1"), true);
#endif

            window.AddSubview(_reCaptchaWebView);
            _reCaptchaWebView.LoadInvisibleCaptcha();

            return _tcsWebView.Task;
        }

        private void RecaptchaWebViewViewControllerOnReCaptchaCompleted(object sender, string recaptchaToken)
        {
            if (!(sender is RecaptchaWebView reCaptchaWebViewViewController))
            {
                return;
            }

            _tcsWebView?.SetResult(recaptchaToken);
            reCaptchaWebViewViewController.ReCaptchaCompleted -= RecaptchaWebViewViewControllerOnReCaptchaCompleted;
            _reCaptchaWebView.Hidden = true;
            _reCaptchaWebView.StopLoading();
            _reCaptchaWebView.RemoveFromSuperview();
            _reCaptchaWebView.Dispose();
            _reCaptchaWebView = null;
        }
    }
}


