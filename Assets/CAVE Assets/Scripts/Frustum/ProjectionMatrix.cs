using CaveAsset.CavePlayer;
using CaveAsset.VirtualScreens;
using System.IO;
using UnityEngine;

namespace CaveAsset
{
	namespace Frustum
	{
		public class ProjectionMatrix : MonoBehaviour
		{
			[Header("Bimber Matrix File Name")]
			[Tooltip("Name of the (.txt) file which contains the bimber matrix")]
			public string bimberMatrixFile = "";
			[Header("Virtual Screen Settings")]
			[Tooltip("Virtual screen for which the projection matrix is to be calculated")]
			public VirtualScreen virtualScreen;

			private Camera cam;
			private CavePlayerController cavePlayerController;

			private Matrix4x4 bimberMatrix = Matrix4x4.identity;
			private Matrix4x4 perspectiveMatrix = Matrix4x4.identity;
			private Matrix4x4 perspectiveMatrixLeftEye = Matrix4x4.identity;
			private Matrix4x4 perspectiveMatrixRightEye = Matrix4x4.identity;

			private GameObject eyeAnchor;
			private GameObject leftEye;
			private GameObject rightEye;

			private void Start()
			{
				cam = GetComponent<Camera>();
				cavePlayerController = GetComponentInParent<CavePlayerController>();

				eyeAnchor = new GameObject("EyeAnchor");
				eyeAnchor.transform.SetParent(transform.parent);
				eyeAnchor.transform.localPosition = Vector3.zero;
				eyeAnchor.transform.localRotation = transform.localRotation;

				leftEye = new GameObject("LeftEye");
				leftEye.transform.SetParent(eyeAnchor.transform);
				leftEye.transform.localPosition = new Vector3(-cam.stereoSeparation / 2.0f, 0.0f, 0.0f);
				leftEye.transform.localRotation = Quaternion.identity;

				rightEye = new GameObject("RightEye");
				rightEye.transform.SetParent(eyeAnchor.transform);
				rightEye.transform.localPosition = new Vector3(cam.stereoSeparation / 2.0f, 0.0f, 0.0f);
				rightEye.transform.localRotation = Quaternion.identity;

				LoadHomographyMatrix();
			}

			private void OnPreCull()
			{
				if (cavePlayerController.testMode)
				{
					perspectiveMatrix = PerspectiveOffCenter(CalculateNearPlane(virtualScreen, transform));

					cam.projectionMatrix = bimberMatrix * perspectiveMatrix;
				}
				else
				{
					eyeAnchor.transform.rotation = transform.rotation;

					perspectiveMatrixLeftEye = PerspectiveOffCenter(CalculateNearPlane(virtualScreen, leftEye.transform));
					perspectiveMatrixRightEye = PerspectiveOffCenter(CalculateNearPlane(virtualScreen, rightEye.transform));

					cam.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, bimberMatrix * perspectiveMatrixLeftEye);
					cam.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, bimberMatrix * perspectiveMatrixRightEye);
				}
			}

			/// <summary>
			/// Creats an Perspectivematrix based on the given NearPlane.
			/// This function comes from the old CAVE Asset.
			/// </summary>
			/// <param name="np">NearPlane</param>
			/// <returns></returns>
			private Matrix4x4 PerspectiveOffCenter(NearPlane np)
			{
				float x = (2.0f * np.near) / (np.right - np.left);
				float y = (2.0f * np.near) / (np.top - np.bottom);
				float a = (np.right + np.left) / (np.right - np.left);
				float b = (np.top + np.bottom) / (np.top - np.bottom);
				float c = -(np.far + np.near) / (np.far - np.near);
				float d = -(2.0f * np.far * np.near) / (np.far - np.near);
				float e = -1.0f;

				Matrix4x4 m = new Matrix4x4();

				m[0, 0] = x;
				m[0, 1] = 0;
				m[0, 2] = a;
				m[0, 3] = 0;
				m[1, 0] = 0;
				m[1, 1] = y;
				m[1, 2] = b;
				m[1, 3] = 0;
				m[2, 0] = 0;
				m[2, 1] = 0;
				m[2, 2] = c;
				m[2, 3] = d;
				m[3, 0] = 0;
				m[3, 1] = 0;
				m[3, 2] = e;
				m[3, 3] = 0;

				return m;
			}

			/// <summary>
			/// Creats a NearPlane based on the given VirtualScreen and transform.
			/// This function comes from the old CAVE Asset.
			/// </summary>
			/// <param name="window">VirtualScreen</param>
			/// <param name="targetTransform">Transform</param>
			/// <returns></returns>
			private NearPlane CalculateNearPlane(VirtualScreen window, Transform targetTransform)
			{
				NearPlane nearPlane = new NearPlane();

				Vector3 nearCenter = targetTransform.position + targetTransform.forward * cam.nearClipPlane;
				Plane plane = new Plane(-targetTransform.forward, nearCenter);
				float distance = 0.0f;
				Vector3 direction;
				Ray ray;

				// calculate top left for nearPlane
				direction = (window.topLeft - targetTransform.position).normalized;
				ray = new Ray(targetTransform.position, direction);
				plane.Raycast(ray, out distance);

				Vector3 nearTopLeft = -(targetTransform.InverseTransformPoint(nearCenter) - targetTransform.InverseTransformPoint((targetTransform.position + direction * distance)));
				nearPlane.left = nearTopLeft.x;
				nearPlane.top = nearTopLeft.y;

				// calculate bottom right for nearPlane
				direction = (window.bottomRight - targetTransform.position).normalized;
				ray = new Ray(targetTransform.position, direction);
				plane.Raycast(ray, out distance);

				Vector3 nearBottomRight = -(targetTransform.InverseTransformPoint(nearCenter) - targetTransform.InverseTransformPoint((targetTransform.position + direction * distance)));
				nearPlane.right = nearBottomRight.x;
				nearPlane.bottom = nearBottomRight.y;

				// near and far clipPlane for nearPlane
				nearPlane.near = cam.nearClipPlane;
				nearPlane.far = cam.farClipPlane;

				return nearPlane;
			}

			/// <summary>
			/// Creats a texture based on the given Quad. The created texture represents
			/// the black frame for the camera. The texture is given to the ScreenOverlay
			/// component. The ScreenOverlay component is Unity standart asset
			/// image-after-effect.
			/// This function comes from the old CAVE Asset.
			/// </summary>
			/// <param name="frame">Quad</param>
			private void CreatePixelMask(Quad frame)
			{
				Texture2D texture = new Texture2D(cam.pixelWidth, cam.pixelHeight);

				Color[] pixels = texture.GetPixels();

				for (int x = 0; x < texture.width; ++x)
				{
					for (int y = 0; y < texture.height; ++y)
					{
						int pos = (y * texture.width) + x;

						pixels[pos] = Color.black;

						Vector2 position = new Vector2((float)x / texture.width, (float)y / texture.height);

						if (frame.Contains(position))
							pixels[pos].a = 0.0f;
					}
				}

				texture.SetPixels(pixels);
				texture.Apply();

				GetComponent<UnityStandardAssets.ImageEffects.ScreenOverlay>().texture = texture;
			}

			/// <summary>
			/// Reads the homographymatrix form the .txt file. Additionally creats the
			/// pixel mask texture for the ScreenOverlay component.
			/// </summary>
			private void LoadHomographyMatrix()
			{
				string matrixFileName = cavePlayerController.pathToBimberMatrix + bimberMatrixFile;
				Matrix4x4 bimber = Matrix4x4.identity;

				try
				{
					bimberMatrix = new Matrix4x4();

					using (StreamReader reader = new StreamReader(new FileStream(matrixFileName, FileMode.Open)))
					{
						for (int i = 0; i < 16; i++)
							bimberMatrix[i] = float.Parse(reader.ReadLine());

						Vector2[] cornorPoints = new Vector2[4];

						for (int i = 0; i < 4; i++)
						{
							cornorPoints[i].x = float.Parse(reader.ReadLine());
							cornorPoints[i].y = float.Parse(reader.ReadLine());

							cornorPoints[i] += Vector2.one;
							cornorPoints[i] /= 2.0f;
						}

						CreatePixelMask(new Quad(cornorPoints));
					}
				}
				catch (System.IO.IsolatedStorage.IsolatedStorageException e)
				{
					Debug.LogException(e);
				}
			}
		}
	}
}
