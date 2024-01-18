using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheakpoint : MonoBehaviour
{
    public Transform[] _points;
    public float _time;

    public CheckpointData _data;

    private void Start()
    {
        _points[_data._index].gameObject.SetActive(true);

        _points[_points.Length - 1].GetComponent<Point>()._last = true;
    }

    public void NextCheckPoint(int _point)
    {
        _points[_data._index].gameObject.SetActive(false);

        _data._times[_data._index] = _time;
        _data._index = _point;
        _data.Save(_point);

        _points[_point].gameObject.SetActive(true);

        _time = 0;
    }

    public void LastCheackPoint(int _point)
    {
        _data._times[_data._index] = _time;
        _data._index = 0;
        _data.Save(_point);
    }

    private void Update()
    {
        _time += Time.deltaTime;
    }
}
