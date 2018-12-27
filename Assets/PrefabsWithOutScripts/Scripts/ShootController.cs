using CaveAsset.Input;
using CaveAsset.Joycon;
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
    public static float speed = 500;
    [Tooltip("Crosshair Prefab")]
    public GameObject crosshairPrefab;
 
    private Queue<GameObject> ballQueue;
    private bool joyMotion = true;

    //INPUT
    private JoyconController joyController;
    private InputController inputController;
    private Joycon joyLeft;
    private Joycon joyRight;
    //Crosshair
    private GameObject leftCrosshair;
    private GameObject rightCrosshair;
    //Offsets
    private float playerOffsetY = 1.2f;

    public void Start()
    {
        ballQueue = new Queue<GameObject>();

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
            if(joyLeft != null)
            {
                leftCrosshair.transform.GetChild(1).transform.Rotate(joyLeft.GetGyroUnity());
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
                if (joyLeft.GetButtonDown(Joycon.Button.DPAD_RIGHT))
                {
                    Reset(joyLeft);
                }
                if (joyLeft.GetButtonDown(Joycon.Button.DPAD_LEFT))
                {
                    joyMotion = !joyMotion;
                }
            }

            if(joyRight != null)
            {
                 rightCrosshair.transform.GetChild(1).transform.Rotate(joyRight.GetGyroUnity());
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
                if (joyRight.GetButtonDown(Joycon.Button.DPAD_LEFT))
                {
                    Reset(joyRight);
                }
                if (joyLeft.GetButtonDown(Joycon.Button.DPAD_RIGHT))
                {
                    joyMotion = !joyMotion;
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

    private void Reset(Joycon joy)
    {
        if(joy != null && joy.isLeft)
        {
            Transform shootdirection = leftCrosshair.transform.GetChild(1).transform;
            shootdirection.forward = transform.forward;
        }
        else if(joy != null)
        {
            Transform shootdirection = rightCrosshair.transform.GetChild(1).transform;
            shootdirection.forward = transform.forward;
        }
    }

    private void ThrowBall(Joycon joy)
    {
        if(ballQueue.Count >= maxBalls)
        {
            GameObject ball = ballQueue.Dequeue();
            ball.GetComponent<TennisballBehaviour>().MakeSound();
            ball.transform.SetPositionAndRotation(GetPlayerBallSpawnPosition(), transform.rotation);
            ball.GetComponent<Rigidbody>().AddForce(GetForce(joy));
            ballQueue.Enqueue(ball);
        }
        else
        {
            GameObject go = Instantiate(Ammunition, GetPlayerBallSpawnPosition(), transform.rotation);
            go.GetComponent<TennisballBehaviour>().MakeSound();
            go.GetComponent<Rigidbody>().AddForce(GetForce(joy));
            ballQueue.Enqueue(go);
        }
    }

    private Vector3 GetPlayerBallSpawnPosition()
    {
        Vector3 position = transform.position;
        //TODO
        //if in cave/with kinect do hand position
        
        //if not in cave do offset
        position.y += playerOffsetY;

        //always add a little forward offset so the player is not pushed back by force
        position += transform.forward;
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
                force = leftCrosshair.transform.GetChild(1).transform.forward;
            }
            else
            {
               force = rightCrosshair.transform.GetChild(1).transform.forward;
            }
        }
        //TODO
        //if in cave
        
        force *= speed;
        return force;
    }
}
