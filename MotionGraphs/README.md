MotionGraphs
============

This is a port of the WWDC2012 sample.

MotionGraphs is an application project that demonstrates a how to use
the push method to receive data from Core Motion. It displays graphs
of accelerometer, gyroscope and device motion data.

The project has the following classes: 

* **GraphView** — A UIView subclass that provides the ability to plot accelerometer, 
gyroscope or device motion data. This is the same GraphView as the 
one in the AccelerometerGraph sample.
* **GraphViewController** — A view controller that handles the display of
accelerometer, gyroscope, and device motion data. Depending on the
argument that is passed into its constructor it can display graph(s)
generated from one of the three data types.
* **AppDelegate** — A standard implementation of the UIApplicationDelegate protocol. 

If you run the compiled application on a device that does not have a
gyroscope, no gyroscope or device motion data will be available. You
cannot effectively run the application on the simulator.

####Ported By: GouriKumari

