using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private DoorManager doorManager;
    private AudioSource audioSource;

    public float openAngle = 90;

    private Transform playerPosition;
    private bool isTriggered = false;
    private bool isOpen = false;
    private float startTime = 0;
    private float startAngle = 0;

    public void Start()
    {
        doorManager = GetComponentInParent<DoorManager>();
        audioSource = GetComponent<AudioSource>();
        playerPosition = doorManager.cavePlayer.transform;

        startAngle = transform.rotation.eulerAngles.y;
    }

    public void Update()
    {
        if (!isTriggered)
        {
            if (!isOpen && Vector3.Distance(transform.position, playerPosition.position) <= doorManager.openingDistance)
                Trigger();

            else if (isOpen && Vector3.Distance(transform.position, playerPosition.position) > doorManager.openingDistance
                        && Time.realtimeSinceStartup - startTime >= doorManager.openTime + doorManager.closeAfterSeconds)
                Trigger();

            return;
        }
        MoveDoor();

    }

    private void MoveDoor()
    {
        float perc = (Time.realtimeSinceStartup - startTime) / doorManager.openTime;
        if (!isOpen)
            perc = 1 - perc;

        perc = Mathf.Clamp01(perc);
        if (perc == 1 || perc == 0)
            isTriggered = false;

        transform.localEulerAngles = new Vector3(0, startAngle + perc * openAngle, 0);
    }

    private void Trigger()
    {
        if (!isTriggered && !isOpen)
        {
            audioSource.clip = doorManager.openingSound;
            audioSource.Play();
        }
        else if (isOpen && !isTriggered)
        {
            audioSource.clip = doorManager.closingSound;
            audioSource.Play();
        }

        isTriggered = true;
        startTime = Time.realtimeSinceStartup;
        isOpen = !isOpen;
    }
}
