
namespace ScanningAndDetecting3DObjects;

internal struct PlaneDrag
{
	internal NMatrix4 PlaneTransform { get; }
	internal NVector3 Offset { get; }

	internal PlaneDrag (NMatrix4 planeTransform, NVector3 offset)
	{
		PlaneTransform = planeTransform;
		Offset = offset;
	}
}
