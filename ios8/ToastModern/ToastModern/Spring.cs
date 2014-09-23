using System;
using Foundation;

namespace ToastModern
{
	public class Spring : NSObject
	{
		public double Position { get; set; }

		public double Damping { get; set; }

		public double Strength { get; set; }

		public double Length { get; set; }

		public double Velocity { get; set; }

		public Spring ()
		{
			Position = 0;
			Damping = 0.9;
			Strength = 0;
			Length = 0;
			Velocity = 0;
		}

		public void Tick ()
		{
			double force = -Strength * (Position - Length);
			Velocity += force;
			Velocity *= Damping;
			Position += Velocity;
		}
	}
}

