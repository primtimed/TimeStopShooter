using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scope : MonoBehaviour
{
    public Gun _gun;

    Camera _cam;

    private void Start()
    {
        _cam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if(_gun._holdingGun)
        {
             _cam.enabled = true;
        }

        else
        {
            _cam.enabled = false;
        }
    }
}
