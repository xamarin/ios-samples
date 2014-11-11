using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Foundation;
using UIKit;

using MonoTouch.Dialog;

namespace InputStreamTest
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		DialogViewController dvc;
		NativeUploader uploader;
		StyledStringElement status;
		StyledStringElement upload;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			dvc = new DialogViewController (new RootElement ("InputStreamTest") 
			{
				new Section () {
					(upload = new StyledStringElement ("Upload", Upload)),
					(status = new StyledStringElement ("")),
				}
			});
			
			upload.Alignment = UITextAlignment.Center;
			status.Alignment = UITextAlignment.Center;
			
			window.RootViewController = dvc;
			window.MakeKeyAndVisible ();
			
			return true;
		}
		
		void Upload ()
		{
			TcpListener listener = new TcpListener (new IPEndPoint (IPAddress.Loopback, 0));
			listener.Start ();
			
			Console.WriteLine ("Listening on: {0}", listener.LocalEndpoint);
			
			uploader = new NativeUploader ();
			uploader.UploadStream ("http://127.0.0.1:" + ((IPEndPoint) listener.LocalEndpoint).Port.ToString (), 1000, () =>
			{
				Console.WriteLine ("Upload completed.");
			});
			
			listener.BeginAcceptSocket ((IAsyncResult res) =>
			{
				ThreadPool.QueueUserWorkItem ((v) => 
				{
					using (var socket = listener.EndAcceptSocket (res)) {
						byte [] buffer = new byte[1024];
						int read;
						
						// receive headers
						read = socket.Receive (buffer);
						BeginInvokeOnMainThread (() => { status.Caption = "Received headers..."; dvc.ReloadData (); });
						Console.WriteLine ("\n" + System.Text.ASCIIEncoding.ASCII.GetString (buffer, 0, read));
						
						// send 100 Continue
						socket.Send (System.Text.ASCIIEncoding.ASCII.GetBytes (@"HTTP/1.1 100 Continue\r\n\r\n"));
						
						// receive data
						read = socket.Receive (buffer);
						BeginInvokeOnMainThread (() => { status.Caption = "Received data"; dvc.ReloadData (); });
						Console.WriteLine ("\n" + System.Text.ASCIIEncoding.ASCII.GetString (buffer, 0, read));
						
						// send 200 OK
						socket.Send (System.Text.ASCIIEncoding.ASCII.GetBytes (@"HTTP/1.1 200 OK\r\n\r\n"));
					}
					
					listener.Stop ();
				});
			}, null);
		}
		
		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}

