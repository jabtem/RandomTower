using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    //현재 화면비

    public float fixedAspectRatioWidth;
    public float fixedAspectRatioHeight;
    Camera cam;
    float fixedaspectratio;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        fixedaspectratio = fixedAspectRatioWidth / fixedAspectRatioHeight;

    }
    private void Start()
    {
        float currentaspectratio = (float)Screen.width / (float)Screen.height;
        if (currentaspectratio == fixedaspectratio)
        {
            return;
        }
        else if (currentaspectratio > fixedaspectratio)
        {
            float w = fixedaspectratio / currentaspectratio;
            float x = (1 - w) * 0.5f;
            cam.rect = new Rect(x, 0.0f, w, 1.0f);
        }
        else if (currentaspectratio < fixedaspectratio)
        {
            float h = currentaspectratio / fixedaspectratio;
            float y = (1 - h) * 0.5f;
            cam.rect = new Rect(0.0f, y, 1.0f, h);
        }

    }

    void OnPreCull() => GL.Clear(true, true, Color.black);
}
