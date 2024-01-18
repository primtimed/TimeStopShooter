using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartNewMap : MonoBehaviour
{
    public GameObject _broken;
    public int _index;

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

        SceneManager.LoadScene(_index);
    }
}
