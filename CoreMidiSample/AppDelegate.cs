using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreMidi;
using MonoTouch.Dialog;
using System.Threading;

namespace CoreMidiSample
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		DialogViewController dvc;
		UIWindow window;
		MidiClient client;
		MidiPort outputPort, inputPort;
		Section hardwareSection;
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();
			
			Midi.Restart ();
			SetupMidi ();
			
			dvc = new DialogViewController (MakeRoot ());
			window.RootViewController = new UINavigationController (dvc);
			return true;
		}

		//
		// Creates the content displayed
		//
		RootElement MakeRoot ()
		{
			return new RootElement ("CoreMidi Sample"){
				(hardwareSection = new Section ("Hardware") {
					MakeHardware (),
					new RootElement ("Devices (" + Midi.DeviceCount + ", " + Midi.ExternalDeviceCount + ")")
				}),
				new Section ("Send"){
					new StringElement ("Send Note", SendNote)	
				},
				new Section ("Commands"){
					new StringElement ("Rescan", delegate { ReloadDevices (); }),
					new StringElement ("Restart MIDI", delegate { Midi.Restart (); }),
				}
			};
		}

		RootElement MakeHardware ()
		{
			int sources = Midi.SourceCount;
			int destinations = Midi.DestinationCount;
			
			return new RootElement ("Endpoints (" + sources + ", " + destinations +")") {
				new Section ("Sources"){
					from x in Enumerable.Range (0, sources)
						let source = MidiEndpoint.GetSource (x)
					    select (Element) new StringElement (source.DisplayName, source.IsNetworkSession ? "Network" : "Local")
				},
				new Section ("Targets"){
					from x in Enumerable.Range (0, destinations)
						let target = MidiEndpoint.GetDestination (x)
					    select (Element) new StringElement (target.DisplayName, target.IsNetworkSession ? "Network" : "Local")
				}
			};
		}
		
		void ReloadDevices ()
		{
			BeginInvokeOnMainThread (delegate {
				hardwareSection.Remove (0);
				hardwareSection.Add (MakeHardware ());
			});
		}
		
		Random rand = new Random ();
		
		void SendNote ()
		{
			for (int i = 0; i < Midi.DestinationCount; i++){
				var endpoint = MidiEndpoint.GetDestination (i);
				
				var note = (byte) (rand.Next () % 127);
				
				// play note
				outputPort.Send (endpoint, new MidiPacket [] { new MidiPacket (0, new byte [] { 0x90, note, 127 })});
				Thread.Sleep (300);
				// turn it off
				outputPort.Send (endpoint, new MidiPacket [] { new MidiPacket (0, new byte [] { 0x80, note, 0 })});
			}
		}
		
		// 
		// Creates a MidiClient which is our way of communicating with the
		// CoreMidi stack
		//
		void SetupMidi ()
		{
			client = new MidiClient ("CoreMidiSample MIDI CLient");
			client.ObjectAdded += delegate(object sender, ObjectAddedOrRemovedEventArgs e) {
				
			};
			client.ObjectAdded += delegate { ReloadDevices (); };
			client.ObjectRemoved += delegate { ReloadDevices (); };
			client.PropertyChanged += delegate(object sender, ObjectPropertyChangedEventArgs e) {
				Console.WriteLine ("Changed");
			};
			client.ThruConnectionsChanged += delegate {
				Console.WriteLine ("Thru connections changed");
			};
			client.SerialPortOwnerChanged += delegate {
				Console.WriteLine ("Serial port changed");
			};
			
			outputPort = client.CreateOutputPort ("CoreMidiSample Output Port");
			inputPort = client.CreateInputPort ("CoreMidiSample Input Port");
			inputPort.MessageReceived += delegate(object sender, MidiPacketsEventArgs e) {
				Console.WriteLine ("Got {0} packets", e.Packets.Length);
			};
			ConnectExistingDevices ();	
			
			var session = MidiNetworkSession.DefaultSession;
			session.Enabled = true;
			session.ConnectionPolicy = MidiNetworkConnectionPolicy.Anyone;
		}

		void ConnectExistingDevices ()
		{
			for (int i = 0; i < Midi.SourceCount; i++){
				var code = inputPort.ConnectSource (MidiEndpoint.GetSource (i));
				if (code != MidiError.Ok)
					Console.WriteLine ("Failed to connect");
			}
		}
		
		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}

