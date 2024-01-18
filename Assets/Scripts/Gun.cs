using Oculus.Interaction;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public bool _auto;

    public int _ammo;
    int _ammoNow;

    public GameObject _obj, _flash, _sound, _cantShoot;
    public Transform _loc;

    public float _shootSpeed;

    float _time;

    InputSystem _input;

    InputAction _shootL, _shootR, _reload;

    bool _shooting = false;
    bool haveAmmo = true;
    bool _reloading = false;

    public bool _shotgun;
    public float _spray, _sprayBullets;

    float _nowBullets;

    public bool _holdingGun, _rechts;

    public Collider _gunCol;

    public Renderer _renderer;

    public Material _material;
    public Material _glitchMaterial;

    private void Awake()
    {
        _input = new InputSystem();
    }

    private void Start()
    {
        _ammoNow = _ammo;
    }

    private void OnEnable()
    {
        _input.Enable();

        _shootR = _input.Movement.TriggerR;
        _shootL = _input.Movement.TriggerL;
        _reload = _input.Movement.Reload;


        _shootR.started += ShootR;
        _shootR.canceled += ShootR;

        _shootL.started += ShootL;
        _shootL.canceled += ShootL;

        _reload.started += Reaload;

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

        if(GameObject.Find("Player").GetComponent<Player>()._dead)
        {
            GetComponent<Grabbable>().EndTransform();
        }

        if(_holdingGun)
        {
            _gunCol.enabled = false;
        }

        else
        {
            _gunCol.enabled = true;
        }

        if (_shooting && _shootSpeed <= _time & _holdingGun && _auto && haveAmmo)
        {
            if (_rechts)
            {
                FeedbackR();
            }

            else
            {
                FeedbackL();
            }

            _ammoNow -= 1;
            Instantiate(_obj, _loc.position, _loc.rotation, null);
            StartCoroutine(Flash());

            _time = 0;
        }

        if (_rechts && !haveAmmo)
        {
            StartCoroutine(FeedbackRCancle());
        }

        else if (!_rechts && !haveAmmo)
        {
            StartCoroutine(FeedbackLCancle());
        }

        if (!haveAmmo)
        {
            StartCoroutine(RealoadAuto());
        }

        else if(haveAmmo && !_shooting)
        {
            //StartCoroutine(RealoadOverTime());
        }

        Ammo();
        StartCoroutine(OutimeUpdate());
    }

    private void ShootR(InputAction.CallbackContext context)
    {
        if (context.started && !haveAmmo)
        {
            Instantiate(_cantShoot, transform);
            return;
        }

        if (context.started && _holdingGun && _shootSpeed <= _time && _rechts && haveAmmo)
        {
            _time = 0;

            if (_auto)
            {
                _shooting = true;

                return;
            }

            _ammoNow -= 1;

            if (_shotgun)
            {
                FeedbackR();
                StartCoroutine(Shotgun());
                StartCoroutine(Flash());
                StartCoroutine(FeedbackRCancle());

                return;
            }

            FeedbackR();
            Instantiate(_obj, _loc.position, _loc.rotation, null);
            StartCoroutine(Flash());
            StartCoroutine(FeedbackRCancle());
        }

        if (context.canceled)
        {
            _shooting = false;
            StartCoroutine(FeedbackRCancle());
        }
    }

    void FeedbackR()
    {
        OVRInput.SetControllerVibration(.1f, 1, OVRInput.Controller.RTouch);
    }

    IEnumerator FeedbackRCancle()
    {
        yield return new WaitForSecondsRealtime(0.005f);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }

    public void ShootL(InputAction.CallbackContext context)
    {
        if(context.started && !haveAmmo)
        {
            Instantiate(_cantShoot, transform);
            return;
        }

        if (context.started && _holdingGun && _shootSpeed <= _time && !_rechts && haveAmmo)
        {
            _time = 0;

            if (_auto)
            {
                _shooting = true;

                return;
            }

            _ammoNow -= 1;

            if (_shotgun)
            {
                FeedbackL();
                StartCoroutine(Shotgun());
                StartCoroutine(Flash());
                StartCoroutine(FeedbackLCancle());

                return;
            }

            FeedbackL();
            Instantiate(_obj, _loc.position, _loc.rotation, null);
            StartCoroutine(Flash());
            StartCoroutine(FeedbackLCancle());
        }

        if (context.canceled)
        {
            _shooting = false;
            StartCoroutine(FeedbackLCancle());
        }
    }

    void FeedbackL()
    {
        OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.LTouch);
    }

    IEnumerator FeedbackLCancle()
    {
        yield return new WaitForSecondsRealtime(0.005f);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
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

    IEnumerator Flash()
    {
        _flash.SetActive(true);
        Instantiate(_sound, _loc.position, _loc.rotation, null);
        yield return new WaitForSecondsRealtime(0.1f);
        _flash.SetActive(false);
    }

    IEnumerator RealoadAuto()
    {
        if(!_reloading)
        {
            _reloading = true;
            yield return new WaitForSeconds(5f);

            _ammoNow = _ammo;
            _reloading = false;
        }
    }

    void Reaload(InputAction.CallbackContext context)
    {
        _ammoNow = _ammo;
    }

    public void Ammo()
    {
        float _perbullet = 256 / _ammo;
        Color _color = new Color(256, _perbullet * _nowBullets, _perbullet * _nowBullets);

        if(_ammoNow <= 0)
        {
            _renderer.material = _glitchMaterial;

            _glitchMaterial.SetColor("_EmissionColor", _color * 1);
            _glitchMaterial.mainTextureScale = new Vector2(Random.Range(1.01f, 1.2f), Random.Range(1.01f, 1.2f));
            haveAmmo = false;
        }

        else
        {
            _renderer.material = _material;

            _material.SetColor("_EmissionColor", Color.white * 1);
            _material.mainTextureScale = new Vector2(1,1);
            haveAmmo = true;
        }
    }

    IEnumerator RealoadOverTime()
    {
        float _time = (_ammo / 5);
        yield return new WaitForSeconds(_time);

        _ammoNow += 1;
    }

}
