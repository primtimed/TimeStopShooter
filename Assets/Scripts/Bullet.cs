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
            GetComponent<Collider>().enabled = false;
            GetComponentInChildren<MeshRenderer>().enabled = false;
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _rb.AddForce(transform.forward * _bulletSpeed, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider collision)
    {
        _hit = true;

        StartCoroutine(Destroy());

        if (collision.gameObject.tag == "Map") { return; }

        Debug.Log("hitenemie");

        if (collision.gameObject.layer == 29)
        {
            if (collision.gameObject.tag == "Head")
            {
                collision.transform.GetComponentInParent<EnemieDead>().HeadShot();
            }

            else if (collision.gameObject.tag == "Enemie")
            {
                collision.transform.GetComponentInParent<EnemieDead>().Dead();
            }
        }

        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("player");
            collision.GetComponentInParent<Player>().Hit(1000);
        }

        StartCoroutine(Forge());
    }

    IEnumerator Forge()
    {
        yield return new WaitForSeconds(.1f);
        GetComponent<Rigidbody>().AddExplosionForce(_explotionForge, transform.position, _explotionRadius, 5);
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

}                                                                 
