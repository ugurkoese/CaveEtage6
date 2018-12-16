using UnityEngine;
using Windows.Kinect;

namespace CaveAsset
{
	namespace Kinect
	{
		public class Floor
		{
			public float X;
			public float Y;
			public float Z;
			public float W;

			public Floor(Windows.Kinect.Vector4 floorClipPlane)
			{
				X = floorClipPlane.X;
				Y = floorClipPlane.Y;
				Z = floorClipPlane.Z;
				W = floorClipPlane.W;
			}

			public float GetHeight()
			{
				return W;
			}

			public float GetTilt()
			{
				return Mathf.Atan(Z / Y) * (180.0f / Mathf.PI);
			}

			public float DistanceFromCameraSpacePoint(CameraSpacePoint point)
			{
				float numerator = X * point.X + Y * point.Y + Z * point.Z + W;
				float denominator = Mathf.Sqrt(X * X + Y * Y + Z * Z);

				return numerator / denominator;
			}
		}
	}
}
