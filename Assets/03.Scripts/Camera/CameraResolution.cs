using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    //현재 화면비
    float currentAspectRatio = (float)Screen.width / (float)Screen.height;
    public float fixedAspectRatioWidth;
    public float fixedAspectRatioHeight;
    Camera cam;
    float fixedAspectRatio;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        fixedAspectRatio = fixedAspectRatioWidth / fixedAspectRatioHeight;
        
    }
    private void Start()
    {
        if(currentAspectRatio == fixedAspectRatio)
        {
            return;
        }
        else if(currentAspectRatio >fixedAspectRatio)
        {
            float w = fixedAspectRatio / currentAspectRatio;
            float x = (1 - w) * 0.5f;
            cam.rect = new Rect(x, 0.0f, w, 1.0f);
        }
        else if(currentAspectRatio < fixedAspectRatio)
        {
            float h = currentAspectRatio / fixedAspectRatio;
            float y = (1 - h) * 0.5f;
            cam.rect = new Rect(0.0f, y, 1.0f, h);
        }
    }

}
