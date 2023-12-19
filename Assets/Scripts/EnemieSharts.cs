using Oculus.Interaction;
using OVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemieSharts : MonoBehaviour
{
    public GameObject _deadSound;

    private void OnCollisionEnter(Collision collision)
    {
        float _int = Random.Range(0, 1);

        if (_int >= .2f)
        {
            Instantiate(_deadSound, transform);
        }
    }
}
