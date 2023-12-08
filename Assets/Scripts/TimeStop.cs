using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeStop : MonoBehaviour
{
    public bool _canPress;

    public float _timeStopActiveTime, _coolDown;
    float _time = 999;

    public Material _material;
    public ParticleSystem _rune;
    public GameObject _ray;


    private void Start()
    {
        
    }

    private void Update()
    {
        _time += Time.deltaTime;

        if(_time > _coolDown)
        {
            _canPress = true;
            _rune.startColor = Color.cyan * 1;
            _material.SetColor("_EmissionColor", Color.cyan * 1);
            _material.mainTextureScale = Vector2.one;
            _ray.SetActive(true);
        }

        else
        {
            _canPress = false;
            _ray.SetActive(false);
            _rune.startColor = Color.red * 1;
            _material.SetColor("_EmissionColor", Color.red * 1);
            _material.mainTextureScale = new Vector2(Random.Range(1.01f, 1.2f), Random.Range(1.01f, 1.2f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if( other.tag == "Hand" && _canPress)
        {
            TimeStopStart();
        }
    }

    void TimeStopStart()
    {
        _ray.SetActive(false);
        Time.timeScale = 0;
        StartCoroutine(StopTimeStop());
    }

    IEnumerator StopTimeStop()
    {
        yield return new WaitForSecondsRealtime(_timeStopActiveTime);
        Time.timeScale = 1;
        _time = 0;
    }
}
