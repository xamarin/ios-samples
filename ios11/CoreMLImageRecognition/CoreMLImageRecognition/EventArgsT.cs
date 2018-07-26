using System;
namespace CoreMLImageRecognition
{
	public class EventArgsT<T> : EventArgs
	{
		public T Value { get; }

		public EventArgsT(T val)
		{
			this.Value = val;
		}
	}

}
