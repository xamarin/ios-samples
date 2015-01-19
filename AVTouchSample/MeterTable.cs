using System;

namespace avTouch
{
	public class MeterTable
	{
		float min_decibels, decibel_resolution, scale_factor;
		float [] table;

		public MeterTable (float inMin) : this (inMin, 400, 2.0f) {}

		double DbToAmp (double db)
		{
			return Math.Pow (10, 0.05  * db);
		}

		public MeterTable (float inMinDecibels, int tableSize, float inRoot)
		{
			if (inMinDecibels >= 0){
				Console.WriteLine ("inMinDecibels Must be negative");
			}

			min_decibels = inMinDecibels;
			decibel_resolution = (float)(min_decibels / (tableSize - 1));
			scale_factor = (float) (1f / decibel_resolution);

			table = new float [tableSize];

			double minAmp = DbToAmp(inMinDecibels);
			double ampRange = 1.0 - minAmp;
			double invAmpRange = 1.0 / ampRange;

			double rroot = 1.0 / inRoot;
			for (int i = 0; i < 	tableSize; ++i) {
				double decibels = i * decibel_resolution;
				double amp = DbToAmp(decibels);
				double adjAmp = (amp - minAmp) * invAmpRange;
				table[i] = (float) Math.Pow (adjAmp, rroot);
			}
		}

		public float ValueAt (float inDecibels)
		{
			if (inDecibels < min_decibels)
				return  0f;
			if (inDecibels >= 0f)
				return 1f;
			int index = (int)(inDecibels * scale_factor);
			return table [index];
		}
	}
}
