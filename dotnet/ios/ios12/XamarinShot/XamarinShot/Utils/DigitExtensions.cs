namespace XamarinShot.Utils;

public static class DigitExtensions
{
        public static float Clamp (float value, float minValue, float maxValue)
        {
                return Math.Min (Math.Max (value, minValue), maxValue);
        }

        public static double Clamp (double value, double minValue, double maxValue)
        {
                return Math.Min (Math.Max (value, minValue), maxValue);
        }

        public static float NormalizedAngle (this float value, float angle, float increment)
        {
                var normalized = value;
                while (Math.Abs (normalized - angle) > increment / 2)
                {
                        if (value > angle)
                        {
                                normalized -= increment;
                        }
                        else
                        {
                                normalized += increment;
                        }
                }

                return normalized;
        }

        /// Returns the remainder of this value divided by the given value using
        /// truncating division.
        ///
        /// Performing truncating division with floating-point values results in a
        /// truncated integer quotient and a remainder. For values `x` and `y` and
        /// their truncated integer quotient `q`, the remainder `r` satisfies
        /// `x == y * q + r`.
        ///
        /// The following example calculates the truncating remainder of dividing
        /// 8.625 by 0.75:
        ///
        ///     let x = 8.625
        ///     print(x / 0.75)
        ///     // Prints "11.5"
        ///
        ///     let q = (x / 0.75).rounded(.towardZero)
        ///     // q == 11.0
        ///     let r = x.truncatingRemainder(dividingBy: 0.75)
        ///     // r == 0.375
        ///
        ///     let x1 = 0.75 * q + r
        ///     // x1 == 8.625
        ///
        /// If this value and `other` are both finite numbers, the truncating
        /// remainder has the same sign as this value and is strictly smaller in
        /// magnitude than `other`. The `truncatingRemainder(dividingBy:)` method
        /// is always exact.
        ///
        /// - Parameter other: The value to use when dividing this value.
        /// - Returns: The remainder of this value divided by `other` using
        ///   truncating division.
        public static double TruncatingRemainder (this double value, double other)
        {
                var q = Math.Round (value / other, MidpointRounding.ToEven);
                return value - (other * q);
        }
}
