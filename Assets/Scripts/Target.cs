using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Target : MonoBehaviour
{
    Vector3 _loc;

    private void Start()
    {
        transform.LookAt(GameObject.Find("Player").transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        _loc = new Vector3(Random.Range(7, -16), Random.Range(30, 0), Random.Range(20, -20));

        Instantiate(gameObject, _loc, Quaternion.identity);

        Destroy(gameObject);
    }
}
