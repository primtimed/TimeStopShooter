using Oculus.Interaction;
using OVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemieSharts : MonoBehaviour
{
    public GameObject _headShot;

    public GameObject _deadSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Damage" || other.transform.tag == "Blade")
        {
            if(tag == "Head" && other.transform.tag != "Blade")
            {
                Instantiate(_headShot, GameObject.Find("Player").transform);
            }

            GetComponentInParent<EnemieDead>().Fall();

            //GetComponentInParent<AIMove>().enabled = false;
            //GetComponentInParent<NavMeshAgent>().enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (GetComponent<Rigidbody>().useGravity == true)
        {
            Instantiate(_deadSound, transform);
        }
    }
}
