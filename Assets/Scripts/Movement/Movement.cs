using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Main Settings")]
    public float _speedAcceleration;
    public float _sensetivitie;
    public float _rotateUpdate;

    [Header("")]
    public BackSettings _back = new BackSettings();

    //private
    InputSystem _input;
    InputAction _move, _rot;

    float _time;

    public Follow _follow;

    [Serializable]
    public class BackSettings
    {
        public Transform _org, _head;
    }

    private void Awake()
    {
        _input = new InputSystem();
    }

    void OnEnable()
    {
        StartCoroutine(OutimeUpdate());

        _input.Enable();

        _move = _input.Movement.Walking;
        _rot = _input.Movement.Rotate;
    }

    void OnDisable()
    {
        StopCoroutine(OutimeUpdate());

        _input.Disable();
    }

    IEnumerator OutimeUpdate()
    {
        yield return new WaitForSecondsRealtime(0.01f);
        Move(_move.ReadValue<Vector2>());
        Rotation(_rot.ReadValue<Vector2>());

        _follow.FollowObj();
        StartCoroutine(OutimeUpdate());
    }

    //movement
    void Move(Vector2 _moveV2)
    {
        if (_moveV2.y != 0 || _moveV2.x != 0)
        {
            Vector3 _moveDirection = (_back._org.transform.forward * _moveV2.y + _back._org.transform.right * _moveV2.x) * Time.unscaledDeltaTime;

            _moveDirection = new Vector3(_moveDirection.x, 0, _moveDirection.z);

            transform.transform.position += _moveDirection * _speedAcceleration;
        }
    }

    void Rotation(Vector2 _moveV2)
    {
        _time += Time.unscaledDeltaTime;

        if (_moveV2.y != 0 || _moveV2.x != 0)
        {
            if(_time > _rotateUpdate)
            {
                _back._head.transform.Rotate(new Vector3(0, _moveV2.x, 0) * _sensetivitie);

                _time = 0;
            }
        }
    }
}
                                                ;