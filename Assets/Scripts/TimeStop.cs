using System.Collections;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using ChromaticAberration = UnityEngine.Rendering.Universal.ChromaticAberration;

public class TimeStop : MonoBehaviour
{
    public bool _canPress;

    public float _timeStopActiveTime, _coolDown;
    float _time = 999;

    public Material _material;
    public ParticleSystem _rune;
    public GameObject _ray;

    Volume _effect;
    ChromaticAberration _ef2;
    ColorCurves _ef1;

    float _timer;


    private void Start()
    {
        _effect = GameObject.Find("Keep").GetComponent<Volume>();
        _effect.profile.TryGet<ChromaticAberration>(out _ef2);
        _effect.profile.TryGet<ColorCurves>(out _ef1);

        _ef2.active = false;
        _ef1.active = false;
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
            StopCoroutine(TimeBeat());
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
        _ef2.active = true;
        _ef1.active = true;
        Time.timeScale = 0;
        StartCoroutine(StopTimeStop());
    }

    IEnumerator StopTimeStop()
    {
        _timer = 2.5f;
        StartCoroutine(TimeBeat());
        yield return new WaitForSecondsRealtime(_timeStopActiveTime);
        Time.timeScale = 1;
        _ef2.active = false;
        _ef1.active = false;
        _time = 0;
    }

    IEnumerator TimeBeat()
    {
        if(Time.timeScale == 0)
        {
            _ef2.active = true;
            _ef1.active = true;
            yield return new WaitForSecondsRealtime(_timer);
            _timer /= 1.5f;
            _ef2.active = false;
            _ef1.active = false;
            yield return new WaitForSecondsRealtime(_timer);

            StartCoroutine(TimeBeat());
        }
    }
}
