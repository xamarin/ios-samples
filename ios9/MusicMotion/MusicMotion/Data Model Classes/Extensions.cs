using CoreMotion;

namespace MusicMotion {
	public static class Extensions {

		public static bool IsSimilarToActivity (this CMMotionActivity activity, CMMotionActivity anotherActivity)
		{
			return activity.Walking && anotherActivity.Walking ||
				activity.Running && anotherActivity.Running ||
				activity.Automotive && anotherActivity.Automotive ||
				activity.Cycling && anotherActivity.Cycling ||
				activity.Stationary && anotherActivity.Stationary;
		}

		public static bool HasActivitySignature (this CMMotionActivity activity)
		{
			return activity.Walking || activity.Running || activity.Automotive || activity.Cycling || activity.Stationary;
		}
	}
}

