using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class EnemieAI : MonoBehaviour
{
    public bool _glitch;

    public EnemieGun _gun;
    bool _aiActive;

    Transform _target;

    NavMeshAgent _agent;

    public float _rotationSpeed;

    float _timer;
    public float _dist;
    float _trackdist;

    Animator _animator;

    [Header("")]
    public Transform[] _points;
    int _index, _newindex;

    private void Start()
    {
        _aiActive = false;

        _target = GameObject.Find("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        if (_points.Length != 0)
        {
            _agent.SetDestination(_points[Random.Range(0, _points.Length)].position);
        }
    }

    private void FixedUpdate()
    {
        float distance = Vector3.Distance(_target.position, transform.position);

        if (distance <= _trackdist)
        {
            _aiActive = true;
            _trackdist = _dist * 2;

            _agent.updateRotation = false;

            Follow();
        }

        else
        {
            _agent.enabled = true;

            _aiActive = false;
            _trackdist = _dist;

            _agent.updateRotation = true;

            StartCoroutine(Idle());

            _animator.SetBool("Aimen", false);
        }

        if (_aiActive)
        {
            LookAt();

            _animator.SetBool("Aimen", true);
        }
    }

    void LookAt()
    {
        _timer += Time.deltaTime;

        if (!_glitch)
        {
            _agent.enabled = false;
        }

        Vector3 direction = (_target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);

        RaycastHit _hit;
        float _random = Random.Range(0.4f, 1.5f);

        if (Physics.SphereCast(_gun.transform.position, .1f, _gun.transform.forward, out _hit, 100))
        {
            //if (_hit.transform.tag == "Player" && _timer > _random)
            //{
            //    Debug.Log("AIShoot");
            //    _gun.Shoot();
            //    _timer = 0;
            //}
        }

        if (Physics.Raycast(_gun.transform.position, _gun.transform.forward, out _hit, 100))
        {
            if (_hit.transform.tag == "Player" && _timer > _random)
            {
                Debug.Log("AIShoot");
                _gun.Shoot();
                _timer = 0;
            }
        }
    }

    IEnumerator Idle()
    {
        if (_agent.remainingDistance < 1 && _points.Length != 0)
        {
            if (_index == _newindex)
            {
                _index = Random.Range(0, _points.Length);
            }

            transform.LookAt(_points[_index].position);

            yield return new WaitForSeconds(1);

            _newindex = _index;
            
            if(_agent.enabled == true)
            {
                _agent.SetDestination(_points[_index].position);
            }
        }
    }

    void Follow()
    {
        if(_agent.enabled == true)
        {
            if (_agent.remainingDistance < 5)
            {

                _agent.SetDestination(transform.position);
            }

            else
            {
                _agent.SetDestination(_target.transform.position);
            }
        }
    }
}
