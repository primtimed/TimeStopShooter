using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCol : MonoBehaviour
{
    public bool _rechts;
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Gun" && other.GetComponent<Gun>() != null)
        {
            if (other.GetComponent<Gun>()._holdingGun == false)
            {
                other.GetComponent<Gun>()._rechts = _rechts;   

                if (other.GetComponentInChildren<GripCheck>() != null)
                {
                    other.GetComponentInChildren<GripCheck>()._rechts = _rechts;
                }
            }
        }
    }
}
