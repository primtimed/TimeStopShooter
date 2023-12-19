using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadEnemie : MonoBehaviour
{
    public void Start()
    {
        StartCoroutine(DeadDelay());
    }

    IEnumerator DeadDelay()
    {
        yield return new WaitForSeconds(3);

        Destroy(gameObject);
    }
}
