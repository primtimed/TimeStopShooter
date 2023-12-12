using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomLight : MonoBehaviour
{
    public Light[] _light;

    private void OnEnable()
    {
        _light[Random.Range(0, _light.Length)].enabled = true;
    }

    private void OnDisable()
    {
        _light[0].enabled = false;
        _light[1].enabled = false;
    }
}
