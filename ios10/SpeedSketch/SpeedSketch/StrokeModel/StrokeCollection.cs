using System.Collections.Generic;

namespace SpeedSketch
{
	public enum StrokePhase
	{
		Began,
		Changed,
		Ended,
		Cancelled
	}

	public class StrokeCollection
	{
		public Stroke ActiveStroke { get; private set; }

		public List<Stroke> Strokes { get; } = new List<Stroke> ();

		void TakeActiveStroke ()
		{
			var stroke = ActiveStroke;
			if (stroke != null) {
				Strokes.Add (stroke);
				ActiveStroke = null;
			}
		}
	}
}
