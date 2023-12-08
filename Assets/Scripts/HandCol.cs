using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCol : MonoBehaviour
{
    public bool _rechts;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Gun" && other.GetComponent<Gun>()._holdingGun == false)
        {
            other.GetComponent<Gun>()._rechts = _rechts;
        }
    }
}
