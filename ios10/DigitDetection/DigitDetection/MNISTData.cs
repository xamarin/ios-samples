using System;
using Foundation;

namespace DigitDetection
{
	public class MNISTData : IDisposable
	{
		byte [] labels;
		byte [] images;

		nuint sizeBias;
		nuint sizeWeights;

		//var hdrW, hdrB: UnsafeMutableRawPointer?
		//var fd_b, fd_w: CInt

		public MNISTData ()
		{
			// get the url to this layer's weights and bias
			var wtPath = NSBundle.MainBundle.PathForResource ("t10k-images-idx3-ubyte", "data");
			var bsPath = NSBundle.MainBundle.PathForResource ("t10k-labels-idx1-ubyte", "data");

			// find and open file
			var URLL = NSBundle.MainBundle.GetUrlForResource ("t10k-labels-idx1-ubyte", "data");
			var dataL = NSData.FromUrl (URLL);

			var URLI = NSBundle.MainBundle.GetUrlForResource ("t10k-images-idx3-ubyte", "data");
			var dataI = NSData.FromUrl (URLI);

			// calculate the size of weights and bias required to be memory mapped into memory
			sizeBias = dataL.Length;
			sizeWeights = dataI.Length;


			// TODO: porting is not completed
			// remove first 16 bytes that contain info data from array
			// images = Array (UnsafeBufferPointer (start: (i + 16), count: sizeWeights - 16))

			// remove first 8 bytes that contain file data from our labels array
			// labels = Array (UnsafeBufferPointer (start: (l + 8), count: sizeBias - 8))
		}

		public void Dispose ()
		{
			throw new NotImplementedException ();
		}
	}
}
