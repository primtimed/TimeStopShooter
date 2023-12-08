using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Follow : MonoBehaviour
{
    public Transform _followObject;

    private void Start()
    {
        StartCoroutine(OutimeUpdate());
    }

    IEnumerator OutimeUpdate()
    {
        yield return new WaitForSecondsRealtime(0.01f);
        FollowObj();

        StartCoroutine(OutimeUpdate());
    }
    public void FollowObj()
    {
        transform.position = new Vector3(_followObject.position.x, transform.position.y, _followObject.position.z);
    }
}
