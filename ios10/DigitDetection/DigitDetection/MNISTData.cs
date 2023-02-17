using System;
using System.IO;
using System.IO.MemoryMappedFiles;

using Foundation;

namespace DigitDetection {
	public class MNISTData : IDisposable {
		public IntPtr Labels { get; private set; }
		public int LabelsCount { get; private set; }

		public IntPtr Images { get; private set; }

		readonly MemoryMappedFile weights;
		readonly MemoryMappedFile biases;

		readonly MemoryMappedViewAccessor wView;
		readonly MemoryMappedViewAccessor bView;

		public MNISTData ()
		{
			// get the url to this layer's weights and bias
			var wtPath = NSBundle.MainBundle.PathForResource ("t10k-images-idx3-ubyte", "data");
			var bsPath = NSBundle.MainBundle.PathForResource ("t10k-labels-idx1-ubyte", "data");

			weights = MemoryMappedFile.CreateFromFile (wtPath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
			biases = MemoryMappedFile.CreateFromFile (bsPath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
			wView = weights.CreateViewAccessor (0, 0, MemoryMappedFileAccess.Read);
			bView = biases.CreateViewAccessor (0, 0, MemoryMappedFileAccess.Read);

			unsafe {
				byte* i = null;
				wView.SafeMemoryMappedViewHandle.AcquirePointer (ref i);
				// remove first 16 bytes that contain info data from array
				Images = ((IntPtr) i) + 16;

				byte* l = null;
				bView.SafeMemoryMappedViewHandle.AcquirePointer (ref l);
				// remove first 8 bytes that contain file data from our labels array
				Labels = (IntPtr) l + 8;
			}
		}

		public void Dispose ()
		{
			wView.SafeMemoryMappedViewHandle.ReleasePointer ();
			wView.Dispose ();
			weights.Dispose ();

			bView.SafeMemoryMappedViewHandle.ReleasePointer ();
			bView.Dispose ();
			biases.Dispose ();
		}
	}
}
