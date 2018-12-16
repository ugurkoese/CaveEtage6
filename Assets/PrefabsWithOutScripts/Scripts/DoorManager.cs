using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Tooltip("Time in seconds to open a door")]
    public float openTime = 2f;
    [Tooltip("Time after the door closes again")]
    public float closeAfterSeconds = 5f;
    [Tooltip("Distance at which the doors open")]
    public float openingDistance = 3f;
    [Tooltip("The Cave Player Object")]
    public GameObject cavePlayer;
    [Tooltip("The sound played when a door is opened")]
    public AudioClip openingSound;
    [Tooltip("The sound played when a door is closed")]
    public AudioClip closingSound;
}
