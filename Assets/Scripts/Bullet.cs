using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody _rb;

    public float _bulletSpeed;

    public float _explotionForge, _explotionRadius;

    bool _hit = false;

    private void Update()
    {
        if (_hit)
        {
            _rb.velocity = new Vector3(0, 0, 0);
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _rb.AddForce(transform.forward * _bulletSpeed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Damage" || collision.gameObject.tag == "Player" || collision.gameObject.tag == "Gun") {return;}

        _hit = true;
        GetComponent<Collider>().enabled = false;
        GetComponentInChildren<MeshRenderer>().enabled = false;

        StartCoroutine(DestroyTime());

        if (collision.gameObject.tag != "Blade")
        {
            return;
        }

        if (collision.gameObject.tag == "Enemie")
        {
            collision.rigidbody.AddExplosionForce(_explotionForge, collision.transform.position, _explotionRadius, 5);
        }

        else if (collision.gameObject.tag != "Map" && collision.gameObject.tag != "Player")
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Gun") { return; }

        _hit = true;
        GetComponent<Collider>().enabled = false;
        GetComponentInChildren<MeshRenderer>().enabled = false;

        StartCoroutine(DestroyTime());

        if (collision.gameObject.tag == "Enemie")
        {
            collision.GetComponent<Rigidbody>().AddExplosionForce(_explotionForge, collision.transform.position, _explotionRadius, 5);
        }

        else if (collision.gameObject.tag != "Map" && collision.gameObject.tag != "Player")
        {
            Destroy(collision.gameObject);
        }
    }

    IEnumerator DestroyTime()
    {
        yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }
}                                                                 
