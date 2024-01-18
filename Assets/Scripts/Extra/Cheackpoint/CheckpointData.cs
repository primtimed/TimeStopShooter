using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckpointData", menuName = "CheckpointData")]
public class CheckpointData : ScriptableObject
{
    public int _index;
    public float[] _times;

    private void Awake()
    {
        _index = PlayerPrefs.GetInt("_index");

        _times[0] = PlayerPrefs.GetFloat("_time" + (0));
        _times[1] = PlayerPrefs.GetFloat("_time" + (1));
        _times[2] = PlayerPrefs.GetFloat("_time" + (2));
        _times[3] = PlayerPrefs.GetFloat("_time" + (3));
        _times[4] = PlayerPrefs.GetFloat("_time" + (4));
    }

    public void Save(int _point)
    {
        PlayerPrefs.SetInt("_index", _point);
        PlayerPrefs.SetFloat("_time" + (_point - 1), _times[_point - 1]);
    }
}
