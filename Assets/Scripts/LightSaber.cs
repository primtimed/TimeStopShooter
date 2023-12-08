using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightSaber : MonoBehaviour
{
    public GameObject _blade;
    public Collider _col;

    Grabbable _holding;

    private void Start()
    {
        _holding = GetComponent<Grabbable>();
    }

    private void Update()
    {
        if (_holding._holding)
        {
            _blade.transform.localScale = Vector3.Lerp(new Vector3(75, 75, 150), _blade.transform.localScale, .95f);
            _blade.GetComponent<Collider>().enabled = true;
            _col.enabled= false;
        }

        else
        {
            _blade.transform.localScale = Vector3.Lerp(_blade.transform.localScale, new Vector3(75, 75, 0), .95f);
            _blade.GetComponent<Collider>().enabled = false;
            _col.enabled= true;
        }
    }
}
