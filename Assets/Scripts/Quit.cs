using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Quit : MonoBehaviour
{
    public GameObject _broken;

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Time());

        Debug.Log("menu");
    }

    IEnumerator Time()
    {
        GetComponent<MeshRenderer>().enabled = false;
        _broken.SetActive(true);

        yield return new WaitForSeconds(5);

        Application.Quit();
    }
}
