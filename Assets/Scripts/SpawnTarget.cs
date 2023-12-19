using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTarget : MonoBehaviour
{
    public GameObject _target;
    GameObject _spawnTarget;
    Vector3 _position;

    private void Update()
    {
        if(_spawnTarget == null)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        _position = new Vector3(transform.position.x + Random.Range(2f, -2f), transform.position.y + Random.Range(1f, -1f), transform.position.z);

        _spawnTarget = Instantiate(_target, _position, transform.rotation);
    }
}
