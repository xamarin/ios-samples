namespace XamarinShot.Models;

// The pose provides positions and tangents along the rope
// as well as upvector information.
// The SlingShotPose further provides methods to interpolate positions,
// tangents or full transforms along the rope.
public class SlingShotPose
{
        int lastIndex = 0;

        /// <summary>
        /// Returns the total length of the slingshot rope
        /// </summary>
        public float TotalLength => Lengths [Lengths.Count - 1];

        public List<SCNVector3> Positions { get; } = new List<SCNVector3> ();

        public List<SCNVector3> Tangents { get; } = new List<SCNVector3> ();

        public List<float> Lengths { get; } = new List<float> ();

        public SCNVector3 UpVector { get; set; } = SCNVector3.UnitY; // Vector used to compute orientation of each segment

        float FindIndex (float l)
        {
                if (l <= 0)
                {
                        lastIndex = 0;
                        return 0f;
                }

                if (l >= TotalLength)
                {
                        lastIndex = Lengths.Count - 2;
                        return 1f;
                }

                while (Lengths [lastIndex] > l)
                {
                        lastIndex -= 1;
                }

                while (Lengths [lastIndex + 1] < l)
                {
                        lastIndex += 1;
                }

                return (l - Lengths [lastIndex]) / (Lengths [lastIndex + 1] - Lengths [lastIndex]);
        }

        /// <summary>
        /// Returns the interpolated position at a given length (l from 0.0 to totalLength)
        /// </summary>
        public SCNVector3 Position (float l)
        {
                var s = FindIndex (l);
                return SimdExtensions.Mix (Positions [lastIndex],
                        Positions [lastIndex + 1], new SCNVector3 (s, s, s));
        }

        /// <summary>
        /// Returns the position for a given index
        /// </summary>
        public SCNVector3 PositionForIndex (int index)
        {
                return Positions [index];
        }

        /// <summary>
        /// Returns the interpolated tangent at a given length (l from 0.0 to totalLength)
        /// </summary>
        public SCNVector3 Tangent (float l)
        {
                var s = FindIndex (l);
                return SCNVector3.Normalize (SimdExtensions.Mix (Tangents [lastIndex],
                        Tangents [lastIndex + 1], new SCNVector3 (s, s, s)));
        }

        /// <summary>
        /// Returns the interpolated transform at a given length (l from 0.0 to totalLength)
        /// </summary>
        public SCNMatrix4 Transform (float l)
        {
                var position = Position (l);
                var x = Tangent (l);
                var y = SCNVector3.Normalize (UpVector);
                var z = SCNVector3.Normalize (SCNVector3.Cross (x, y));
                y = SCNVector3.Normalize (SCNVector3.Cross (z, x));

                var rot = new OpenTK.Matrix3 (x.X, y.X, z.X, x.Y, y.Y, z.Y, x.Z, y.Z, z.Z);
                var matrix = SCNMatrix4.Rotate (rot.ToQuaternion ());
                return matrix.SetTranslation ((OpenTK.Vector3)position);
        }

        /// <summary>
        /// Returns the distance to the previous segment for a given index
        /// </summary>
        public float DistanceToPrev (int index)
        {
                return Lengths [index] - Lengths [index - 1];
        }

        /// <summary>
        /// Returns the distance to the next segment for a given index
        /// </summary>
        public float DistanceToNext (int index)
        {
                return Lengths [index + 1] - Lengths [index];
        }
}
