using System;
using MonoTouch.HealthKit;

namespace Fit
{
	public interface IHealthStore
	{
		HKHealthStore HealthStore { get; set; }
	}
}

