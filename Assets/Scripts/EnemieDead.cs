using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemieDead : MonoBehaviour
{
    public GameObject _deadEnemie, _newWeapon;

    public GameObject _headshotSound;
    public Transform _weaponLoc;
    bool _activated;

    public void HeadShot()
    {
        if (!_activated)
        {
            _activated = true;
            Instantiate(_headshotSound, transform.position, transform.rotation);

            Dead();
        }
    }

    public void Dead()
    {
        //falling enemie
        Instantiate(_newWeapon, _weaponLoc.position, _weaponLoc.rotation);
        Instantiate(_deadEnemie, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
