using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemieSharts : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Damage" || other.transform.tag == "Blade")
        {
            GetComponentInParent<EnemieDead>().Fall();
        }
    }
}
