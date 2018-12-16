using CaveAsset.Input;
using JoyconAPI;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    [Tooltip("Prefab for the Ammunition")]
    public GameObject Ammunition;
    [Tooltip("The sound playing when a ball is thrown")]
    public GameObject ThrowSound;
    [Tooltip("Define how many balls should persist")]
    public int maxBalls = 300;
    [HideInInspector]
    public static float speed = 500;

    private Queue<GameObject> ballQueue;

    //INPUT
    private JoyconManager manager;
    private List<Joycon> joyconList;
    private Joycon[] joycons;
    private InputController inputController;

    //orientation of controller
    private Vector3 gyro;
    private Vector3 accel;
    private Vector3 orientation;

    public void Start()
    {
        ballQueue = new Queue<GameObject>();
        gyro = new Vector3(0, 0, 0);
        accel = new Vector3(0, 0, 0);
        orientation = transform.forward;

        //INPUT INIT
        //JOYCONS
        manager = JoyconManager.Instance;
        joyconList = manager.joycons;
        joycons = new Joycon[joyconList.Count];
        for (int i = 0; i < joyconList.Count; i++)
        {
            joycons[i] = joyconList[i];
            Debug.Log(joycons[i]);
        }
        //INPUT CONTROLLER(for mouse and keyboard)
        inputController = GetComponent<InputController>();
    }

    public void Update()
    {
        
        if (inputController.enableMouseAndKeyboard)
        {// do mouse and keyboard
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ThrowBall();
            }
        }
        else
        {//do joycons
            foreach (Joycon joycon in joycons)
            {
                if (joycon.GetButtonDown(Joycon.Button.SHOULDER_2))
                {
                    joycon.SetRumble(160.0f, 320.0f, 0.6f, 150);
                    ThrowBall();
                }
                if(joycon.GetButtonDown(Joycon.Button.DPAD_DOWN))
                {
                    foreach(GameObject ball in ballQueue)
                    {
                        ball.GetComponent<TennisballBehaviour>().SwitchLight();
                    }
                }
            }
        }
    }

    private void ThrowBall()
    {
        if(ballQueue.Count >= maxBalls)
        {
            GameObject ball = ballQueue.Dequeue();
            ball.GetComponent<TennisballBehaviour>().MakeSound();
            ball.transform.SetPositionAndRotation(GetPlayerBallSpawnPosition(), transform.rotation);
            ball.GetComponent<Rigidbody>().AddForce(GetForce());
            ballQueue.Enqueue(ball);
        }
        else
        {
            GameObject go = Instantiate(Ammunition, GetPlayerBallSpawnPosition(), transform.rotation);
            go.GetComponent<TennisballBehaviour>().MakeSound();
            go.GetComponent<Rigidbody>().AddForce(GetForce());
            ballQueue.Enqueue(go);
        }
    }

    private Vector3 GetPlayerBallSpawnPosition()
    {
        Vector3 position = transform.position;
        //TODO
        //if in cave/with kinect do hand position
        
        //if not in cave do offset
        position.y += 1.2f;

        //always add a little forward offset so the player is not pushed back by force
        position += transform.forward;
        return position;
    }

    private Vector3 GetForce()
    {
        Vector3 force = transform.forward;
        //TODO
        //if in cave
        //position der hand herausfinden (links, rechtshänder?)

        //if at home
        force.y = 0.3f;
        force *= speed;
        return force;
    }
}
