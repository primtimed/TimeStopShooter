using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public float _DestroyTime;

    private void Start()
    {
        StartCoroutine(Stop());  
    }

    IEnumerator Stop()
    {
        yield return new WaitForSecondsRealtime(_DestroyTime);

        Destroy(gameObject);
    }
}
