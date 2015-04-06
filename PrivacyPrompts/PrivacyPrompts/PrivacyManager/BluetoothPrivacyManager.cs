using System;
using System.Threading.Tasks;
using CoreBluetooth;
using UIKit;

namespace PrivacyPrompts
{
	public class BluetoothPrivacyManager : IPrivacyManager, IDisposable
	{
		readonly CBCentralManager cbManager = new CBCentralManager ();

		public Task RequestAccess ()
		{
			if (cbManager.State == CBCentralManagerState.Unauthorized)
				cbManager.ScanForPeripherals (new CBUUID [0]);

			return Task.FromResult<object> (null);
		}

		public string CheckAccess ()
		{
			return cbManager.State.ToString ();
		}

		public void Dispose ()
		{
			cbManager.Dispose ();
		}
	}
}