using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemieDead : MonoBehaviour
{
    Rigidbody[] _rb;

    public GameObject _deadSound;

    private void Start()
    {
        _rb = GetComponentsInChildren<Rigidbody>();
    }

    public void Fall()
    {
        foreach (var rb in _rb)
        {
            rb.useGravity = true;
        }

        StartCoroutine(DestroyTime());
    }

    IEnumerator DestroyTime()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
