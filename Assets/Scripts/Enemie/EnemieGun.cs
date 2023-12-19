using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemieGun : MonoBehaviour
{
    public GameObject _bullet, _flash, _sound;

    public Transform _loc;

    public void Shoot()
    {
        Instantiate(_bullet, _loc.position, _loc.rotation, null);
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        _flash.SetActive(true);
        Instantiate(_sound, _loc.position, _loc.rotation, null);
        yield return new WaitForSecondsRealtime(0.1f);
        _flash.SetActive(false);
    }
}
