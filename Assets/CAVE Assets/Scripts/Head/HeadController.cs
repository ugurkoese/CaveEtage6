using CaveAsset.CavePlayer;
using CaveAsset.Input;
using CaveAsset.Kinect;
using UnityEngine;

namespace CaveAsset
{
	namespace Head
	{
		public class HeadController : MonoBehaviour
		{
			private CavePlayerController cavePlayerController = null;
			private KinectController kinectController = null;

			private void Start()
			{
				cavePlayerController = GetComponentInParent<CavePlayerController>();

				if (!cavePlayerController.testMode)
					kinectController = GetComponentInParent<InputController>().GetKinectController();
			}

			private void Update()
			{
				if (!cavePlayerController.testMode)
				{
					if (kinectController.IsNewFrame())
					{
						Vector3 newHeadPosition = kinectController.GetJointPosition(Windows.Kinect.JointType.Head);

						if (newHeadPosition != Vector3.zero)
							transform.localPosition = newHeadPosition;
					}
				}
			}
		}
	}
}
