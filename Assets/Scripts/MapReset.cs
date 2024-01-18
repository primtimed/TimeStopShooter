using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapReset : MonoBehaviour
{
    public int _index;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Hand")
        {
            SceneManager.LoadScene(_index);
        }
    }
}
