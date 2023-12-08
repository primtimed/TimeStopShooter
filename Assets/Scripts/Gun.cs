using Oculus.Interaction;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public bool _auto;

    public GameObject _obj;
    public Transform _loc;

    public float _shootSpeed;

    float _time;

    InputSystem _input;

    InputAction _shootL, _shootR;

    bool _shooting = false;

    public bool _shotgun;
    public float _spray, _sprayBullets;

    float _nowBullets;

    public bool _holdingGun, _rechts;

    public Collider _gunCol;

    private void Awake()
    {
        _input = new InputSystem();
    }

    private void OnEnable()
    {
        _input.Enable();

        _shootR = _input.Movement.TriggerR;
        _shootL = _input.Movement.TriggerL;


        _shootR.started += ShootR;
        _shootR.canceled += ShootR;

        _shootL.started += ShootL;
        _shootL.canceled += ShootL;

        StartCoroutine(OutimeUpdate());
    }

    private void OnDisable()
    {
        _input.Disable();

        StopCoroutine(OutimeUpdate());
    }

    IEnumerator OutimeUpdate()
    {
        yield return new WaitForSecondsRealtime(0.233333333333333f);

        _time += Time.unscaledDeltaTime;

        _holdingGun = GetComponent<Grabbable>()._holding;

        if(_holdingGun)
        {
            //_gunCol.enabled = false;
        }

        else
        {
            //_gunCol.enabled = true;
        }

        if (_shooting && _shootSpeed <= _time & _holdingGun && _auto)
        {
            Instantiate(_obj, _loc.position, _loc.rotation, null);
            _time = 0;
        }

        StartCoroutine(OutimeUpdate());
    }

    private void ShootR(InputAction.CallbackContext context)
    {
        if (context.started && _holdingGun && _shootSpeed <= _time && _rechts)
        {
            _time = 0;

            if (_auto)
            {
                _shooting = true;

                return;
            }

            if (_shotgun)
            {
                StartCoroutine(Shotgun());

                return;
            }

            Instantiate(_obj, _loc.position, _loc.rotation, null);
        }

        if (context.canceled)
        {
            _shooting = false;
        }
    }

    private void ShootL(InputAction.CallbackContext context)
    {
        if (context.started && _holdingGun && _shootSpeed <= _time && !_rechts)
        {
            _time = 0;

            if (_auto)
            {
                _shooting = true;

                return;
            }

            if (_shotgun)
            {
                StartCoroutine(Shotgun());

                return;
            }

            Instantiate(_obj, _loc.position, _loc.rotation, null);
        }

        if (context.canceled)
        {
            _shooting = false;
        }
    }

    IEnumerator Shotgun()
    {
        while (_nowBullets < _sprayBullets)
        {
            yield return new WaitForSecondsRealtime(0.001f);

            Vector3 _bloom;

            _bloom.x = Random.Range(-_spray, _spray);
            _bloom.y = Random.Range(-_spray, _spray);
            _bloom.z = 0;

            _loc.transform.localRotation = Quaternion.Euler(_bloom);

            Instantiate(_obj, _loc.position, _loc.rotation, null);

            _nowBullets++;
        }

        _nowBullets = 0;
    }
}
