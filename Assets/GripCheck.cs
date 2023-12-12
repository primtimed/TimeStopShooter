using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GripCheck : MonoBehaviour
{
    public GameObject _handR, _handL;

    public Grabbable _gun;

    public bool _rechts, _canGrip;

    InputSystem _inputSystem;

    InputAction _gripR, _gripL;
    GameObject _lookat;

    private void Awake()
    {
        _inputSystem = new InputSystem();
    }

    private void OnEnable()
    {
        _gripL = _inputSystem.Movement.HoldL;
        _gripR = _inputSystem.Movement.HoldR;

        _gripL.performed += GrapL;
        _gripR.performed += GrapR;
    }

    private void OnDisable()
    {
        _gripR.performed -= GrapR;
        _gripL.performed -= GrapL;
    }

    private void OnCollisionEnter(Collision other)
    {
        _canGrip = true;
        _lookat = other.gameObject;
    }

    private void OnCollisionExit(Collision other)
    {
        _canGrip = false;
        _lookat = null;
    }

    private void Update()
    {
        if(_lookat != null && (_handL.active || _handL.active))
        {
            _gun.gameObject.transform.LookAt(_lookat.transform.position);
        }
    }

    private void GrapL(InputAction.CallbackContext context)
    {
        if(!_rechts)
        {
            _handL.SetActive(true);
        }

        else
        {
            _handL.SetActive(false);
        }
    }

    private void GrapR(InputAction.CallbackContext context)
    {
       if(_rechts)
       {
            _handR.SetActive(true);
       }

        else
        {
            _handR.SetActive(false);
        }
    }
}
