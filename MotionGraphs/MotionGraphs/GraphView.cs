using System;
using System.Collections.Generic;
using CoreGraphics;
using UIKit;
using Foundation;
using CoreGraphics;
using CoreAnimation;

namespace MotionGraphs
{
	// The GraphView class needs to be able to update the scene
	// quickly in order to track the data at a fast enough frame
	// rate. The naive implementation tries to draw the entire
	// graph every frame, but unfortunately that is too much
	// content to sustain a high framerate. As such this class
	// uses CALayers to cache previously drawn content and
	// arranges them carefully to create an illusion that we are
	// redrawing the entire graph every frame.
	//
	// Functions used to draw all content
	[Register ("GraphView")]
	public class GraphView : UIView
	{
		CGPoint kSegmentInitialPosition = new CGPoint (14.0f, 56.0f);

		List<GraphViewSegment> segments { get; set; }

		GraphTextView  text { get; set; }

		GraphViewSegment current { get; set; }

		public void CommonInit ()
		{
			// Create the text view and add it as a subview. We keep a weak reference
			// to that view afterwards for laying out the segment layers.
			text = new GraphTextView (new CGRect (0.0f, 0.0f, 32.0f, 112.0f));
			AddSubview (text);

			// Create a mutable array to store segments, which is required by -addSegment
			segments = new List<GraphViewSegment> ();

			// Create a new current segment, which is required by -addX:y:z and other methods.
			// This is also a weak reference (we assume that the 'segments' array will keep the strong reference).
			current = AddSegment ();
		}

		public GraphView (IntPtr handle) : base (handle)
		{
			CommonInit ();
		}

		public GraphView ()
		{
			CommonInit ();
		}

		public GraphView (CGRect frame) : base (frame)
		{
			CommonInit ();
		}

		public GraphView (NSCoder coder) : base (coder)
		{
			CommonInit ();
		}

		public static CGColor CreateDeviceGrayColor (float w, float a)
		{
			using (var gray = CGColorSpace.CreateDeviceGray ()) {
				return new CGColor (gray, new nfloat[] { w, a });
			}
		}

		public static CGColor CreateDeviceRGBColor (float r, float g, float b, float a)
		{
			using (var rgb = CGColorSpace.CreateDeviceRGB ()) {
				return new CGColor (rgb, new nfloat [] { r, g, b, a });
			}
		}

		public static CGColor GraphBackgroundColour ()
		{
			return CreateDeviceGrayColor (0.6f, 1.0f);
		}

		public static CGColor GraphLineColor ()
		{
			return CreateDeviceGrayColor (0.5f, 1.0f);
		}

		public static CGColor GraphXColor ()
		{
			return CreateDeviceRGBColor (1.0f, 0.0f, 0.0f, 1.0f);
		}

		public static CGColor GraphYColor ()
		{
			return CreateDeviceRGBColor (0.0f, 1.0f, 0.0f, 1.0f);
		}

		public static CGColor GraphZColor ()
		{
			return CreateDeviceRGBColor (0.0f, 0.0f, 1.0f, 1.0f);
		}

		public static void DrawGridLines (CGContext context, float x, float width)
		{
			for (float y = -48.5f; y <= 48.5f; y += 16.0f) {
				context.MoveTo (x, y);
				context.AddLineToPoint (x + width, y);
			}
			context.SetStrokeColor (GraphLineColor ());
			context.StrokePath ();
		}

		public void AddX (double x, double y, double z)
		{
			// First, add the new value to the current segment
			if (current.AddX (x, y, z)) {
				// If after doing that we've filled up the current segment, then we need to
				// determine the next current segment
				RecycleSegment ();
				// And to keep the graph looking continuous, we add the value to the new segment as well.
				current.AddX (x, y, z);
			}
			// After adding a new data point, we need to advance the x-position of all the segment layers by 1 to
			// create the illusion that the graph is advancing.
			foreach (GraphViewSegment gSegment in segments) {
				CGPoint position = gSegment.Layer.Position;
				position.X += 1.0f;
				gSegment.Layer.Position = position;
			}
		}
		// The initial position of a segment that is meant to be displayed on the left side of the graph.
		// This positioning is meant so that a few entries must be added to the segment's history before it becomes
		// visible to the user. This value could be tweaked a little bit with varying results, but the X coordinate
		// should never be larger than 16 (the center of the text view) or the zero values in the segment's history
		// will be exposed to the user.
		GraphViewSegment AddSegment ()
		{
			// Create a new segment and add it to the segments array.
			GraphViewSegment segment = new GraphViewSegment (); 
			segments.Insert (0, segment);
			//Ensure that newly added segment layers are placed after the text view's layer so that the text view
			// always renders above the segment layer.
			Layer.InsertSublayerBelow (segment.Layer, text.Layer);
			// Console.WriteLine (this.Layer.InsertSublayerBelow (segment.layer, text.Layer));
			// Position it properly 
			//segment.layer.Position = kSegmentInitialPosition;
			segment.Layer.Position = kSegmentInitialPosition;
			return segment;
		}

		void RecycleSegment ()
		{
			// We start with the last object in the segments array, as it should either be visible onscreen,
			// which indicates that we need more segments, or pushed offscreen which makes it eligable for recycling.
			int lastIndex = segments.Count - 1;
			GraphViewSegment lastObj = segments [lastIndex];
			//Console.WriteLine(Layer.Bounds);
			if (lastObj.IsVisibleInRect (Layer.Bounds)) {
				current = AddSegment ();
			} else {
				lastObj.Reset ();
				lastObj.Layer.Position = kSegmentInitialPosition;
				segments.Insert (0, lastObj);
				segments.Remove (lastObj);
				//And make it our current segment
				current = lastObj;
			}
		}
		// The graph view itself exists only to draw the background and gridlines. All other content is drawn either into
		// the GraphTextView or into a layer managed by a GraphViewSegment.
		public override void Draw (CGRect rect)
		{
			using (var context = UIGraphics.GetCurrentContext ()) {
				//Fill in the background			
				context.SetFillColor (GraphBackgroundColour ());
				context.FillRect (Bounds);

				float width = (float)Bounds.Size.Width;
				context.TranslateCTM (0.0f, 56.0f);

				//Draw the grid lines
				DrawGridLines (context, 0.0F, width);

				UIColor.White.SetColor ();
			}
		}
		// We use a seperate view to draw the text for the
		// graph so that we can layer the segment layers below
		// it which gives the illusion that the numbers are
		// drawn over the graph, and hides the fact that the
		// graph drawing for each segment is incomplete until
		// the segment is filled.
		class GraphTextView : UIView
		{
			public GraphTextView (CGRect rect) : base (rect)
			{
			}

			UIFont systemFont = UIFont.SystemFontOfSize (12.0f);

			void DrawLabel (string label, float pos)
			{
				DrawString (label, new CGRect (2, pos, 24, 16), systemFont, UILineBreakMode.WordWrap, UITextAlignment.Right);
			}

			public override void Draw (CGRect rect)
			{
				using (var context = UIGraphics.GetCurrentContext ()) {
			
					//Fill in the background			
					context.SetFillColor (GraphBackgroundColour ());
					context.FillRect (Bounds);
					context.TranslateCTM (0.0f, 56.0f);		
			
					//Draw the gridlines			
					DrawGridLines (context, 26.0f, 6.0f);
			
					//Draw the text			
					UIColor.White.SetColor ();			
					DrawLabel ("+3.0", -56.0f);
					DrawLabel ("+2.0", -40.0f);
					DrawLabel ("+1.0", -24.0f);
					DrawLabel ("0.0", -8.0f);                      
					DrawLabel ("-1.0", 8.0f);
					DrawLabel ("-2.0", 24.0f);
					DrawLabel ("-3.0", 40.0f);
				}
			}
		}
		// The GraphViewSegment manages up to 32 values and a CALayer that it updates with
		// the segment of the graph that those values represent.
		class GraphViewSegment
		{
			public CALayer Layer { get; set; }
			// Need 33 values to fill 32 pixel width.
			double[] xhistory = new double [33];
			double[] yhistory = new double [33];
			double[] zhistory = new double [33];
			int index;

			public GraphViewSegment ()
			{
				Layer = new CALayer ();
				Layer.Delegate = new LayerDelegate (this);
				Layer.Bounds = new CGRect (0.0f, -56.0f, 32.0f, 112.0f);
				Layer.Opaque = true;
				index = 33;
			}

			void Clear (double[] array)
			{
				for (int i = 0; i < array.Length; i++)
					array [i] = 0;
			}

			public void Reset ()
			{
				Clear (xhistory);
				Clear (yhistory);
				Clear (zhistory);
				index = 33;
				Layer.SetNeedsDisplay ();
			}

			public bool IsFull ()
			{
				return index == 0;
			}

			public bool IsVisibleInRect (CGRect r)
			{
				// Returns true if the layer for this segment is visible in the given rect.
				return r.IntersectsWith (Layer.Frame);
			}

			public bool AddX (double x, double y, double z)
			{
				// If this segment is not full, then we add a new value to the history.
				if (index > 0) {
					// First decrement, both to get to a zero-based index and to flag one fewer position left
					--index;
					xhistory [index] = x;
					yhistory [index] = y;
					zhistory [index] = z;
					// And inform Core Animation to redraw the layer.
					Layer.SetNeedsDisplay ();
				}
				// And return if we are now full or not (really just avoids needing to call isFull after adding a value).
				return index == 0;				
			}

			class LayerDelegate : CALayerDelegate
			{
				GraphViewSegment _parent;

				public LayerDelegate (GraphViewSegment parent)
				{
					_parent = parent; 
				}

				public override void DrawLayer (CALayer layer, CGContext context)
				{
					// Fill in the background
					//GraphView gView = new GraphView ();
					context.SetFillColor (GraphBackgroundColour ());
					context.FillRect (layer.Bounds);
					
					// Draw the grid lines
					DrawGridLines (context, 0.0f, 32.0f);
					
					//Draw the graph
					CGPoint[] lines = new CGPoint[64];
					int i;
										
					//X
					for (i = 0; i < 32; ++i) {	   
						lines [i * 2].X = i;
						lines [i * 2].Y = ((float)(_parent.xhistory [i] * (-1)) * 16.0f);
						lines [(i * 2 + 1)].X = i + 1;
						lines [(i * 2 + 1)].Y = ((float)(_parent.xhistory [i + 1] * (-1)) * 16.0f);
					}

					context.SetStrokeColor (GraphXColor ());
					context.StrokeLineSegments (lines);
				
					//Y
					for (i = 0; i < 32; ++i) {
						lines [i * 2].Y = ((float)(_parent.yhistory [i] * (-1)) * 16.0f);
						lines [(i * 2 + 1)].Y = ((float)(_parent.yhistory [i + 1] * (-1)) * 16.0f);
					}
					
					context.SetStrokeColor (GraphYColor ());
					context.StrokeLineSegments (lines);

					//Z
					for (i = 0; i < 32; ++i) {
						lines [i * 2].Y = ((float)(_parent.zhistory [i] * (-1)) * 16.0f);
						lines [(i * 2 + 1)].Y = ((float)(_parent.zhistory [i + 1] * (-1)) * 16.0f);
					}
					
					context.SetStrokeColor (GraphZColor ());
					context.StrokeLineSegments (lines);
				}

				public override NSObject ActionForLayer (CALayer layer, string eventKey)
				{
					return NSNull.Null;
				}
			}
		}
	}
}