namespace ScanningAndDetecting3DObjects;

// See README.MD 
internal class SimpleBox<T> : Foundation.NSObject
{
	internal T Value { get; }

	internal SimpleBox (T v)
	{
		this.Value = v;
	}
}
