using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public int _pointNumber = 1;

    public bool _last;

    private void OnTriggerEnter(Collider other)
    {
        if (_last)
        {
            GetComponentInParent<Cheakpoint>().LastCheackPoint(_pointNumber);
            return;
        }

        GetComponentInParent<Cheakpoint>().NextCheckPoint(_pointNumber);
    }
}
