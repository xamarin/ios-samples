using System;
using CoreGraphics;
using System.Collections.Generic;
using Foundation;
using CoreMotion;
using UIKit;

namespace MotionGraphs
{
	public enum MotionDataType
	{
		AccelerometerData,
		GyroData,
		DeviceMotion 
	}

	public enum DeviceMotionGraphType
	{
		Attitude,
		RotationRate,
		Gravity,
		UserAcceleration		
	}

	public partial class GraphViewController : UIViewController
	{
		MotionDataType graphDataSource;
		List<string> graphTitles;
		List<GraphView>graphs;
		CMMotionManager mManager;
		const double accelerometrMin = 0.01;
		const double gyroMin = 0.01;
		const double deviceMotionMin = 0.01;

		public GraphViewController (string title, MotionDataType type)
		{
			Title = title;
			graphDataSource = type;
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			//Delegate has to be called
			AppDelegate appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
			mManager = appDelegate.SharedManager;
			UpdateIntervalSlider.Value = 0.0f;
			if (graphDataSource != MotionDataType.DeviceMotion) {
				SegmentedControl.Hidden = true;
			} else {
				graphTitles = new List<string> { "Motion.Attitude", "Motion.RotationRate", "Motion.Gravity", "Motion.UserAcceleration" };
				graphs = new List<GraphView> { 
					primaryGraph, 						// attitudeGraph
					new GraphView (primaryGraph.Frame), // rotationRateGraph
					new GraphView (primaryGraph.Frame), // gravityGraph
					new GraphView (primaryGraph.Frame)	// userAccelerationGraph
				};
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			StartUpdatesWithMotionDataType (graphDataSource, (int)(UpdateIntervalSlider.Value * 100));
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			StopUpdatesWithMotionType (graphDataSource);
		}

		partial void OnSliderValueChanged (UISlider sender)
		{
			StartUpdatesWithMotionDataType (graphDataSource, (int)(sender.Value * 100));			
		}

		partial void SegmentedControlDidChanged (UISegmentedControl sender)
		{
			GraphView newView = graphs [(int)sender.SelectedSegment];
			primaryGraph.RemoveFromSuperview ();
			View.AddSubview (newView);
			primaryGraph = newView;
			primaryGraphLabel.Text = graphTitles [(int)sender.SelectedSegment];
		}

		public void SetLabelValueX (double x, double y, double z)
		{
			xLabel.Text = String.Format (" x: {0:0.000}", x);
			yLabel.Text = String.Format (" y: {0:0.000}", y);
			zLabel.Text = String.Format (" z: {0:0.000}", z);
		}

		public void SetLabelValueRoll (double roll, double pitch, double yaw)
		{
			xLabel.Text = String.Format (" roll: {0:0.000}", roll);
			yLabel.Text = String.Format (" pitch: {0:0.000}", pitch);
			zLabel.Text = String.Format (" yaw: {0:0.000}", yaw);
		}

		public void StartUpdatesWithMotionDataType (MotionDataType type, int sliderValue)
		{
			double updateInterval = 0.00;
			double delta = 0.005;
			switch (graphDataSource) {
			case MotionDataType.AccelerometerData:
				updateInterval = GraphViewController.accelerometrMin + delta * sliderValue;
				if (mManager.AccelerometerAvailable) {
					mManager.AccelerometerUpdateInterval = updateInterval;
					mManager.StartAccelerometerUpdates (NSOperationQueue.CurrentQueue, ( data,  error) => { 
						if (primaryGraph == null)
							return;

						primaryGraph.AddX (data.Acceleration.X, data.Acceleration.Y, data.Acceleration.Z);
						SetLabelValueX (data.Acceleration.X, data.Acceleration.Y, data.Acceleration.Z);
					});
				}
				primaryGraphLabel.Text = "AccelerometerData.Acceleration";
				break;			
			case MotionDataType.GyroData:
				updateInterval = gyroMin + delta * sliderValue;
				if (mManager.GyroAvailable) {
					mManager.GyroUpdateInterval = updateInterval;
					mManager.StartGyroUpdates (NSOperationQueue.CurrentQueue, (gyroData, error) => {
						if (primaryGraph == null)
							return;

						primaryGraph.AddX (gyroData.RotationRate.x, gyroData.RotationRate.y, gyroData.RotationRate.z);
						SetLabelValueX (gyroData.RotationRate.x, gyroData.RotationRate.y, gyroData.RotationRate.z);
					});
				}
				primaryGraphLabel.Text = "GyroData.RotationRate";
				break;
			case MotionDataType.DeviceMotion:
				updateInterval = deviceMotionMin + delta * sliderValue;
				if (mManager.DeviceMotionAvailable) {
					mManager.DeviceMotionUpdateInterval = updateInterval;
					mManager.StartDeviceMotionUpdates (NSOperationQueue.CurrentQueue, (motion, error) => {
			
						graphs [(int)DeviceMotionGraphType.Attitude].AddX (motion.Attitude.Roll, motion.Attitude.Pitch, motion.Attitude.Yaw);
						graphs [(int)DeviceMotionGraphType.RotationRate].AddX (motion.RotationRate.x, motion.RotationRate.y, motion.RotationRate.z);
						graphs [(int)DeviceMotionGraphType.Gravity].AddX (motion.Gravity.X, motion.Gravity.Y, motion.Gravity.Z);
						graphs [(int)DeviceMotionGraphType.UserAcceleration].AddX (motion.UserAcceleration.X, motion.UserAcceleration.Y, motion.UserAcceleration.Z);
					   
						switch ((DeviceMotionGraphType) (int)SegmentedControl.SelectedSegment) {
						case DeviceMotionGraphType.Attitude:
							SetLabelValueRoll (motion.Attitude.Roll, motion.Attitude.Pitch, motion.Attitude.Yaw);
							break;
						case DeviceMotionGraphType.RotationRate:
							SetLabelValueX (motion.RotationRate.x, motion.RotationRate.y, motion.RotationRate.z);
							break;
						case DeviceMotionGraphType.Gravity:
							SetLabelValueX (motion.Gravity.X, motion.Gravity.Y, motion.Gravity.Z);
							break;
						case DeviceMotionGraphType.UserAcceleration:
							SetLabelValueX (motion.UserAcceleration.X, motion.UserAcceleration.Y, motion.UserAcceleration.Z);
							break;
						}	
					});
				}
				primaryGraphLabel.Text = graphTitles [(int)SegmentedControl.SelectedSegment]; 
				break;
			}		
			UpdateIntervalLabel.Text = updateInterval.ToString ();
		}

		public void StopUpdatesWithMotionType (MotionDataType type)
		{
			switch (graphDataSource) {
			case MotionDataType.AccelerometerData:
				if (mManager.AccelerometerActive)
					mManager.StopAccelerometerUpdates ();
				break;
			case MotionDataType.GyroData:
				if (mManager.GyroActive)
					mManager.StopGyroUpdates ();
				break;
			case MotionDataType.DeviceMotion:
				if (mManager.DeviceMotionActive)
					mManager.StopDeviceMotionUpdates ();
				break;
			}
		}
	}
}