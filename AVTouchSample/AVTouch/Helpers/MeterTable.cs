using System;

namespace AVTouch
{
    /// <summary>
    /// Class for handling conversion from linear scale to dB
    /// </summary>
    public class MeterTable
    {
        private readonly float minDecibels;
        private readonly float decibelResolution;
        private readonly float scaleFactor;
        private readonly float[] table;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AVTouch.MeterTable"/> class.
        /// </summary>
        /// <param name="inMinDecibels">The decibel value of the minimum displayed amplitude.</param>
        /// <param name="inTableSize">The size of the table. The table needs to be large enough that there are no large gaps in the response.</param>
        /// <param name="inRoot">This controls the curvature of the response. 2.0 is square root, 3.0 is cube root. But inRoot doesn't have to be integer valued, it could be 1.8 or 2.5, etc.</param>
        public MeterTable(float inMinDecibels = -80f, int inTableSize = 400, float inRoot = 2f)
        {
            if (inMinDecibels >= 0)
            {
                Console.WriteLine($"MeterTable {nameof(inMinDecibels)} must be negative");
            }

            minDecibels = inMinDecibels;
            decibelResolution = (minDecibels / (inTableSize - 1));
            scaleFactor = 1f / decibelResolution;

            table = new float[inTableSize];

            var minAmp = DbToAmp(minDecibels);
            var ampRange = 1d - minAmp;
            var invAmpRange = 1d / ampRange;

            var rroot = 1.0 / inRoot;
            for (int i = 0; i < inTableSize; ++i)
            {
                var decibels = i * decibelResolution;
                var amp = DbToAmp(decibels);
                var adjAmp = (amp - minAmp) * invAmpRange;
                table[i] = (float)Math.Pow(adjAmp, rroot);
            }
        }

        public float ValueAt(float inDecibels)
        {
            float result;
            if (inDecibels < minDecibels)
            {
                result = 0f;
            }
            else if (inDecibels >= 0f)
            {
                result = 1f;
            }
            else
            {
                var index = (int)(inDecibels * scaleFactor);
                result = table[index];
            }

            return result;
        }

        private double DbToAmp(double db)
        {
            return Math.Pow(10d, 0.05d * db);
        }
    }
}