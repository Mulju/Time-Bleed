using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera camera;

    void Update()
    {
        if(camera == null) camera = Camera.main;

        transform.LookAt(camera.transform);
    }
}
