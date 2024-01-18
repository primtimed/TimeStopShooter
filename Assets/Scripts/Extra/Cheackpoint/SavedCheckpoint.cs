using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SavedCheckpoint : MonoBehaviour
{
    public int _checkpointId;

    public TextMeshProUGUI _text;
    float _time;
    int _min;

    private void Start()
    {
        _time = PlayerPrefs.GetFloat("_time" + (_checkpointId));

        TimeM();

        _text.text = _min.ToString() + ":" + _time.ToString("F2");
    }

    private void TimeM()
    {
        if(_time > 60)
        {
            _min += 1;
            _time -= 60;
            TimeM();
        }
    }
}
