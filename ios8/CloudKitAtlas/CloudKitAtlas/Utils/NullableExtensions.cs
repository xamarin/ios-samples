using System;

namespace CloudKitAtlas
{
	public static class NullableExtensions
	{
		public static T GetValueOrDefault<T> (this T? value, T defaultValue = default(T)) where T : struct
		{
			return ValueOrDefault (value, defaultValue);
		}

		public static T ValueOrDefault<T> (T? value, T defaultValue = default (T)) where T : struct
		{
			return value.HasValue ? value.Value : defaultValue;
		}
	}
}
