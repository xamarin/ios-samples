using System.Collections.Generic;

namespace SpeedSketch
{
	public class StrokeCollection
	{
		public Stroke ActiveStroke { get; set; }

		public List<Stroke> Strokes { get; } = new List<Stroke> ();

		public void TakeActiveStroke ()
		{
			var stroke = ActiveStroke;
			if (stroke != null) {
				Strokes.Add (stroke);
				ActiveStroke = null;
			}
		}
	}
}
