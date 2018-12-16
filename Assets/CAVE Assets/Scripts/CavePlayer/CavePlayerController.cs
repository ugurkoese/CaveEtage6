using CaveAsset.Head;
using UnityEngine;

namespace CaveAsset
{
	namespace CavePlayer
	{
		public class CavePlayerController : MonoBehaviour
		{
			[Header("Test Mode Settings")]
			[Tooltip("Should be used for local development. Turn on to disable the Kinect and Wii functions and use the local bimber matrix files. NOTE: NEED BE BE TURNED OFF BEFOR BUILDING AND DEPLOYING THE PROJECT FOR THE CAVE!")]
			public bool testMode = false;
			[Tooltip("Path to folder where the bimber matrix files are located")]
			[Header("Bimber Matrix Path")]
			public string pathToBimberMatrix = "";

			private const string pathToBimberMatrixForTestMode = "Y:/cave/cfg/BimberMatrizen/";

			private HeadController headController;
			private CapsuleCollider capsuleCollider;

			private void Awake()
			{
				if (testMode)
					pathToBimberMatrix = pathToBimberMatrixForTestMode;
			}

			private void Start()
			{
				headController = GetComponentInChildren<HeadController>();
				capsuleCollider = GetComponent<CapsuleCollider>();

				CalculateCollider();
			}

			private void Update()
			{
				CalculateCollider();
			}

			/// <summary>
			/// Calculates the CapsuleCollider component based on the heads position.
			/// The CapsuleCollider component is used by the unity engine for collision detection.
			/// </summary>
			private void CalculateCollider()
			{
				Vector3 newHeadPosition = headController.transform.localPosition;

				if (newHeadPosition != Vector3.zero)
				{
					newHeadPosition.y /= 2.0f;
					newHeadPosition.y += capsuleCollider.radius / 2.0f;

					capsuleCollider.center = newHeadPosition;
					capsuleCollider.height = newHeadPosition.y * 2.0f;
				}
			}
		}
	}
}
