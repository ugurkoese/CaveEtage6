using CaveAsset.Input;
using CaveAsset.Head;
using CaveAsset.Joycon;
using CaveAsset.Kinect;
using JoyconAPI;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    [Tooltip("Prefab for the Ammunition")]
    public GameObject Ammunition;
    //[Tooltip("The sound playing when a ball is thrown")]
    //public GameObject ThrowSound;
    [Tooltip("Define how many balls should persist")]
    public int maxBalls = 300;
    [HideInInspector]
    public static float speed = 600;
    [Tooltip("Crosshair Prefab")]
    public GameObject crosshairPrefab;

    public HeadController head;

    private Transform headtransform;

    private Queue<GameObject> ballQueue;
    private bool joyMotion = true;

    //INPUT
    private JoyconController joyController;
    private InputController inputController;
    private Joycon joyLeft;
    private Joycon joyRight;
    //Kinect
    private KinectController kinectController;
    //Crosshair
    private GameObject leftCrosshair;
    private GameObject rightCrosshair;
    //Offsets
    private float playerOffsetY = 1.2f;

    public void Start()
    {
        ballQueue = new Queue<GameObject>();
        headtransform = head.GetComponent<Transform>();
        //INPUT INIT
        //JOYCONS
        joyController = GetComponent<JoyconController>();
        //INPUT CONTROLLER(for mouse and keyboard)
        inputController = GetComponent<InputController>();
        //Grab the joycons only if the input is not mouse and keyboard
        if (!inputController.enableMouseAndKeyboard)
        {
            joyLeft = joyController.GetLeftJoycon();
            joyRight = joyController.GetRightJoycon();
            leftCrosshair = Instantiate(crosshairPrefab, transform);
            leftCrosshair.transform.position = new Vector3(0, -100, 0);
            rightCrosshair = Instantiate(crosshairPrefab, transform);
            rightCrosshair.transform.position = new Vector3(0, -100, 0);
        }
        kinectController = GetComponent<KinectController>();
    }

    public void Update()
    {
        if (inputController.enableMouseAndKeyboard)
        {// do mouse and keyboard
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ThrowBall(null);
            }
        }
        else
        {//do joycons
            //Aim
            if (joyLeft != null)
            {
                Vector3 leftElbow = kinectController.GetJointPosition(Windows.Kinect.JointType.ElbowLeft);
                Vector3 leftWrist = kinectController.GetJointPosition(Windows.Kinect.JointType.WristLeft);
                Vector3 direction = Vector3.Normalize(leftWrist - leftElbow);
                direction = headtransform.TransformDirection(direction);
                leftCrosshair.transform.GetChild(1).transform.forward = direction;

                if (joyLeft.GetButton(Joycon.Button.SHOULDER_1))
                {
                    PlaceLaser(joyLeft, leftCrosshair);
                }
                if (joyLeft.GetButtonUp(Joycon.Button.SHOULDER_1))
                {
                    leftCrosshair.transform.position = new Vector3(0, -100, 0);
                }
                //Shoot
                if (joyLeft.GetButtonDown(Joycon.Button.SHOULDER_2))
                {

                    joyLeft.SetRumble(160.0f, 320.0f, 0.6f, 150);
                    ThrowBall(joyLeft);
                }
            }

            if (joyRight != null)
            {
                Vector3 rightElbow = kinectController.GetJointPosition(Windows.Kinect.JointType.ElbowRight);
                Vector3 rightWrist = kinectController.GetJointPosition(Windows.Kinect.JointType.WristRight);
                Vector3 direction = Vector3.Normalize(rightWrist - rightElbow);
                //direction = rightCrosshair.transform.InverseTransformDirection(direction);
                direction = headtransform.TransformDirection(direction);
                rightCrosshair.transform.GetChild(1).transform.forward = direction;
                //rightCrosshair.transform.eulerAngles = new Vector3(rightCrosshair.transform.rotation.x, transform.eulerAngles.y, rightCrosshair.transform.rotation.z);
                if (joyRight.GetButton(Joycon.Button.SHOULDER_1))
                {
                    PlaceLaser(joyRight, rightCrosshair);
                }
                if (joyRight.GetButtonUp(Joycon.Button.SHOULDER_1))
                {
                    rightCrosshair.transform.position = new Vector3(0, -100, 0);
                }
                //Shoot
                if (joyRight.GetButtonDown(Joycon.Button.SHOULDER_2))
                {

                    joyRight.SetRumble(160.0f, 320.0f, 0.6f, 150);
                    ThrowBall(joyRight);
                }
            }
        }
    }

    private void PlaceLaser(Joycon joy, GameObject crosshair)
    {
        Transform shootdirection = crosshair.transform.GetChild(1).transform;
        RaycastHit hit;
        Vector3 start = transform.position;
        start.y += playerOffsetY;
        start += shootdirection.forward;
        if (Physics.Raycast(start, shootdirection.forward, out hit, Mathf.Infinity))
        {
            crosshair.transform.GetChild(0).transform.rotation = Quaternion.Euler(hit.normal);
            crosshair.transform.position = hit.point;
        }
    }

    private void ThrowBall(Joycon joy)
    {
        if (ballQueue.Count >= maxBalls)
        {
            GameObject ball = ballQueue.Dequeue();
            ball.GetComponent<TennisballBehaviour>().Disabled = false;
            ball.GetComponent<TennisballBehaviour>().MakeSound();
            ball.transform.SetPositionAndRotation(GetPlayerBallSpawnPosition(joy), transform.rotation);
            ball.GetComponent<Rigidbody>().AddForce(GetForce(joy));
            ballQueue.Enqueue(ball);
        }
        else
        {
            GameObject go = Instantiate(Ammunition, GetPlayerBallSpawnPosition(joy), transform.rotation);
            go.GetComponent<TennisballBehaviour>().MakeSound();
            go.GetComponent<Rigidbody>().AddForce(GetForce(joy));
            ballQueue.Enqueue(go);
        }

    }

    private Vector3 GetPlayerBallSpawnPosition(Joycon joy)
    {
        Vector3 position = transform.position;

        //if not in cave do offset
        position.y += playerOffsetY;

        //always add a little forward offset so the player is not pushed back by force
        position += transform.forward;
        if(joy != null)
        {
            if (joy.isLeft)
            {
                position.x += -0.1f;
            }
            else
            {
                position.x += 0.1f;
            }

        }

        return position;
    }

    private Vector3 GetForce(Joycon joy)
    {
        Vector3 force;
        force = transform.forward;
        force.y = 0.3f;
        if (joyMotion && joy != null)
        {
            if (joy.isLeft)
            {
                //Get the vector from your elbow to your wrist and take that as the direction of the force
                force = leftCrosshair.transform.GetChild(1).transform.forward;
            }
            else
            {
                //Get the vector from your elbow to your wrist and take that as the direction of the force
                force = rightCrosshair.transform.GetChild(1).transform.forward;
            }
        }

        force *= speed;
        return force;
    }
}
