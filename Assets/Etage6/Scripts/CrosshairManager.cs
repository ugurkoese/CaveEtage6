using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    [Tooltip("Color of the crosshair")]
    public Color crosshairColor = new Color(0f, 0f, 1f);
    [Tooltip("Transparency of the Crosshair. 1 = Opaque, 0 = Invisible")]
    [Range(0, 1)]
    public float transparency = 0.7f;
    [Tooltip("The size of the dot")]
    [Range(1, 100)]
    public float size = 1f;

    // Start is called before the first frame update
    void Start()
    {
        crosshairColor.a = transparency;
        //Grab the renderer from the child
        GetComponentInChildren<Renderer>().material.SetColor("_Color", crosshairColor);
        Vector3 currentScale = GetComponentInChildren<Transform>().localScale;
        GetComponentInChildren<Transform>().localScale = new Vector3(currentScale.x * size, currentScale.y * size, currentScale.z);
    }


    public void SetCrosshairColor()
    {
        GetComponentInChildren<Renderer>().material.SetColor("_Color", crosshairColor);
    }
}
