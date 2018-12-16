using UnityEngine;

namespace CaveAsset
{
	namespace VirtualScreens
	{
		[ExecuteInEditMode]
		public class VirtualScreen : MonoBehaviour
		{
			[Header("Screen Size")]
			[Tooltip("Size of the screen in meter (width x height)")]
			public Vector2 size = new Vector3(3.0f, 3.0f);

			[HideInInspector]
			public Vector3 topLeft;
			[HideInInspector]
			public Vector3 topRight;
			[HideInInspector]
			public Vector3 bottomRight;
			[HideInInspector]
			public Vector3 bottomLeft;

			private Vector3[] origPoints = new Vector3[4];
			private Vector3 lastPosition;
			private Quaternion lastRotation;

			private void OnDrawGizmos()
			{
				Gizmos.DrawLine(topLeft, topRight);
				Gizmos.DrawLine(topRight, bottomRight);
				Gizmos.DrawLine(bottomRight, bottomLeft);
				Gizmos.DrawLine(bottomLeft, topLeft);
			}

			private void Start()
			{
				origPoints[0] = new Vector3(-size.x / 2.0f, size.y / 2.0f, 0.0f);
				origPoints[1] = new Vector3(size.x / 2.0f, size.y / 2.0f, 0.0f);
				origPoints[2] = new Vector3(size.x / 2.0f, -size.y / 2.0f, 0.0f);
				origPoints[3] = new Vector3(-size.x / 2.0f, -size.y / 2.0f, 0.0f);
			}

			private void Update()
			{
				if (transform.position != lastPosition || transform.rotation != lastRotation)
				{
					topLeft = transform.TransformPoint(origPoints[0]);
					topRight = transform.TransformPoint(origPoints[1]);
					bottomRight = transform.TransformPoint(origPoints[2]);
					bottomLeft = transform.TransformPoint(origPoints[3]);

					lastPosition = transform.position;
					lastRotation = transform.rotation;
				}
			}
		}
	}
}
