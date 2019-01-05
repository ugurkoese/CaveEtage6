using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    private enum EnemyType
    {
        standing,
        rotating,
        dropping
    }

    [SerializeField]
    private EnemyType enemyType = EnemyType.standing;
    public Texture normalTexture;
    public Texture hitTexture;
    private Vector3 startPosition;
    private bool readyToHit;
    [Range(1, 5)]
    public float speed = 3f;
    [Range(3, 60)]
    public float reactivationInterval = 3;
    private Vector3 startRotation;
    private Vector3 targetRotation;
    private Vector3 targetStandUp;
    private AudioSource audioSource;

    public void Start()
    {
        targetStandUp = new Vector3(transform.localEulerAngles.x + 80, transform.localEulerAngles.y, transform.localEulerAngles.z);
        startRotation = transform.localEulerAngles;
        targetRotation = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + 180, transform.localEulerAngles.z);
        startPosition = transform.position;
        readyToHit = true;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.2f;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Tennisball(Clone)" && readyToHit && !(other.GetComponent<TennisballBehaviour>().Disabled))
        {
            audioSource.Play();
            readyToHit = false;
            transform.Find("Front").GetComponent<Renderer>().material.SetTexture("_MainTex", hitTexture);
        }
    }

    private void Reactivate()
    {
        readyToHit = true;
        transform.Find("Front").GetComponent<Renderer>().material.SetTexture("_MainTex", normalTexture);
    }

    // Update is called once per frame
    void Update()
    {
        switch (enemyType)
        {
            case EnemyType.dropping:
                if (readyToHit && transform.position.y > 0)
                {
                    transform.position += (-transform.up) * speed * Time.deltaTime;
                }
                else if (!readyToHit && transform.position.y < startPosition.y)
                {
                    transform.position += transform.up * speed * Time.deltaTime;
                    if (transform.position.y >= startPosition.y)
                    {
                        Invoke("Reactivate", reactivationInterval);
                    }
                }
                break;
            case EnemyType.rotating:
                if (readyToHit) //startrotation.y = 150
                {
                    if (transform.localEulerAngles.y < targetRotation.y)
                    {
                        transform.RotateAround(transform.position, transform.up, Time.deltaTime * 30 * speed);
                    }
                }
                else if (!readyToHit)
                {
                    if (transform.localEulerAngles.y > startRotation.y)
                    {
                        transform.RotateAround(transform.position, -transform.up, Time.deltaTime * 30 * speed);
                        if (transform.localEulerAngles.y <= startRotation.y)
                        {
                            Invoke("Reactivate", reactivationInterval);
                        }
                    }
                }
                break;
            case EnemyType.standing:
                if (readyToHit)
                {
                    if (gameObject.name == "EnemyS2")
                    {
                    }
                    if (transform.localEulerAngles.x < 350f)
                    {
                        transform.Rotate(Vector3.right * Time.deltaTime * speed * 30);
                    }
                }
                else if (!readyToHit)
                {
                    if (transform.localEulerAngles.x <= 275f)
                    {
                        Invoke("Reactivate", reactivationInterval);
                    }
                    else if (transform.localEulerAngles.x > 275f)
                    {
                        transform.Rotate(-Vector3.right * Time.deltaTime * speed * 30, Space.Self);
                    }

                    
                }
                break;
        }
    }

}
