using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisballBehaviour : MonoBehaviour
{
    public AudioSource audioSource;

    private bool disabled = false;
    public bool Disabled
    {
        get
        {
            return disabled;
        }

        set
        {
            disabled = value;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Disabled = true;
    }

    // private AudioSource audioSource;
    public void Start()
    {
        SwitchLight();
    }
    public void MakeSound()
    {
        audioSource.Play();
    }

    public void SwitchLight()
    {
        Light light = transform.GetChild(0).GetComponent<Light>();
        if (transform.childCount > 0)
        {
            if (light.isActiveAndEnabled)
            {
                light.enabled = false;
            }
            else
            {
                light.enabled = true;
            }
        }
    }
}
