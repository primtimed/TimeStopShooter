using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class EnemieAI : MonoBehaviour
{
    public EnemieGun _gun;
    bool _aiActive;

    Transform _target;

    public float _rotationSpeed;

    float _timer;
    float _trackdist = 8;

    private void Start()
    {
        _aiActive = false;

        _target = GameObject.Find("Player").transform;
    }

    private void FixedUpdate()
    {
        float distance = Vector3.Distance(_target.position, transform.position);

        if (distance <= _trackdist)
        {
            _aiActive = true;
            _trackdist = 15;
        }

        else
        {
            _aiActive = false;
            _trackdist = 8;
        }

        if (_aiActive)
        {
            LookAt();
        }
    }

    void LookAt()
    {
        _timer += Time.deltaTime;

        Vector3 direction = (_target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);

        RaycastHit _hit;
        float _random = Random.Range(0.4f, 1.5f);

        if (Physics.Raycast(_gun.transform.position, _gun.transform.forward, out _hit, 15))
        {
            if (_hit.transform.tag == "Player" && _timer > _random)
            {
                Debug.Log("AIShoot");
                _gun.Shoot();
                _timer = 0;
            }
        }
    }
}
