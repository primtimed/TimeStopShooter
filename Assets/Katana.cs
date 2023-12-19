using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 29)
        {
            other.transform.GetComponentInParent<EnemieDead>().Dead();
        }
    }
}
