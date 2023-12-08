using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scope : MonoBehaviour
{
    public Gun _gun;

    public Camera _cam;

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
