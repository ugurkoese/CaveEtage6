using CaveAsset.CavePlayer;
using CaveAsset.Kinect;
//using CaveAsset.Wii;
using CaveAsset.Joycon;
using UnityEngine;

namespace CaveAsset
{
    namespace Input
    {
        public class InputController : MonoBehaviour
        {
            [Header("Input Typs")]
            [Tooltip("Enables the classic input way with WSAD and mouse look")]
            public bool enableMouseAndKeyboard = false;

            [Header("Input Sensitivity")]
            [Tooltip("Walking speed modifier")]
            public float walkingSpeed = 5.0f;
            [Tooltip("Turning speed modifier")]
            public float turningSpeed = 3.0f;

            private CavePlayerController cavePlayerController = null;
            private KinectController kinectController = null;
            private JoyconController joyconController = null;
            //private WiiController wiiController = null;

            private void Awake()
            {
                cavePlayerController = GetComponent<CavePlayerController>();
                joyconController = GetComponent<JoyconController>();
                kinectController = GetComponent<KinectController>();
                //wiiController = GetComponent<WiiController>();

                if (cavePlayerController.testMode)
                {
                    kinectController.enabled = false;
                    joyconController.enabled = false;
                    //wiiController.enabled = false;
                }
            }

            private void Update()
            {
                if (enableMouseAndKeyboard)
                    MouseAndKeyboardMovement();

                if (!cavePlayerController.testMode)
                {
                    JoyconMovment();
                    PushButton();
                    //WiimoteMovement();

                    /*if (wiiController.GetWiimoteButtonHold(WiiRemoteButton.HOME))
						Application.Quit();*/
                }
            }

            /// <summary>
            /// Performs the movement based on the input that comes from mouse and keyboard.
            /// W, Arrow up -> moving forward
            /// S, Arrow down -> moving backward
            /// A, Arrow left -> moving left
            /// D, Arrow right -> moving right
            /// Mouse input -> turning up/down/left/right
            /// </summary>
            private void MouseAndKeyboardMovement()
            {
                float strafe = UnityEngine.Input.GetAxis("Horizontal") * Time.deltaTime * walkingSpeed;
                float forwardBackward = UnityEngine.Input.GetAxis("Vertical") * Time.deltaTime * walkingSpeed;
                float mouseX = UnityEngine.Input.GetAxis("Mouse X") * turningSpeed;

                transform.Translate(strafe, 0.0f, forwardBackward);
                transform.Rotate(0.0f, mouseX, 0.0f);
            }

            /// <summary>
            /// Performs the movement based on the input that comes from the Wiimote.
            /// Cross up -> moving forward
            /// Cross down -> moving backward
            /// Cross left -> truning left
            /// Cross right -> turning right
            /// </summary>
            /*private void WiimoteMovement()
			{
				float rotateY = 0.0f;
				float forwardBackward = 0.0f;

				if (wiiController.GetWiimoteButtonHold(WiiRemoteButton.CROSS_UP))
					forwardBackward = Time.deltaTime * walkingSpeed;
				if (wiiController.GetWiimoteButtonHold(WiiRemoteButton.CROSS_DOWN))
					forwardBackward = -1.0f * Time.deltaTime * walkingSpeed;
				if (wiiController.GetWiimoteButtonHold(WiiRemoteButton.CROSS_RIGHT))
					rotateY = turningSpeed;
				if (wiiController.GetWiimoteButtonHold(WiiRemoteButton.CROSS_LEFT))
					rotateY = -1.0f * turningSpeed;

				transform.Translate(0.0f, 0.0f, forwardBackward);
				transform.Rotate(0.0f, rotateY, 0.0f);
			}*/

            /// <summary>
            /// Performs the movement based on the input that comes from the Joycon.
            /// When two Joycons are connected:
            /// Left Stick up -> moving forward
            /// Left Stick down -> moving backward
            /// Left Stick left -> strafing left
            /// Left Stick right -> strafing right
            /// Right Stick up -> look up
            /// Right Stick down -> look down
            /// Right Stick left -> turning left
            /// Right Stick right -> turning right
            /// 
            /// When one Joycon is connected:
            /// Stick up -> moving forward
            /// Stick down -> moving backward
            /// Stick left -> turning left
            /// Stick right -> turning right
            /// </summary>
            private void JoyconMovment()
            {
                float[] move;
                float[] look;
                float distance = Time.deltaTime * walkingSpeed;
                //float rotateX;
                float rotateY;

                if (joyconController.GetNumberOfJoycons() > 1)
                {
                    move = joyconController.GetLeftJoycon().GetStick();
                    look = joyconController.GetRightJoycon().GetStick();
                    //rotateX = (-1 * look[1]) * turningSpeed;
                    rotateY = look[0] * turningSpeed;

                    transform.Translate(Vector3.right * (move[0] * distance));
                    transform.Translate(Vector3.forward * (move[1] * distance));

                    transform.Rotate(0.0f, rotateY, 0.0f);
                    //transform.Rotate(rotateX, 0.0f, 0.0f);
                }
                else
                {
                    move = joyconController.GetLeftJoycon().GetStick();
                    // move[0] is the X-Axis of the joycon if holded vertical
                    rotateY = move[0] * turningSpeed;

                    // move[1] is the Y-Axis of the joycon if holded vertical
                    transform.Translate(Vector3.forward * (move[1] * distance));

                    transform.Rotate(0.0f, rotateY, 0.0f);
                }
            }

            private void PushButton()
            {
                if (joyconController.GetNumberOfJoycons() > 1)
                {
                    if (joyconController.GetJoycon(1).GetButtonDown(JoyconAPI.Joycon.Button.DPAD_UP))
                        Application.Quit();
                }
                else
                {
                    if (joyconController.GetJoycon(0).GetButtonDown(JoyconAPI.Joycon.Button.DPAD_UP))
                        Application.Quit();
                }
            }

            /// <summary>
            /// Returns the KinectController component. When in test mode returns null.
            /// </summary>
            /// <returns>KinectController component or null when in test mode</returns>
            public KinectController GetKinectController()
            {
                if (cavePlayerController.testMode)
                    return null;
                else
                    return kinectController;
            }

            /// <summary>
            /// Returns the WiiController component. When in test mode returns null.
            /// </summary>
            /// <returns>WiiController component or null when in test mode</returns>
            /*public WiiController GetWiiController()
			{
				if (cavePlayerController.testMode)
					return null;
				else
					return wiiController;
			}*/

            /// <summary>
            /// Returns the WiiController component. When in test mode returns null.
            /// </summary>
            /// <returns>JoyconController component or null when in test mode</returns>
            public JoyconController GetJoyconController()
            {
                if (cavePlayerController.testMode)
                    return null;
                else
                    return joyconController;
            }
        }
    }
}
