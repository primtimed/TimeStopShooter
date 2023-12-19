using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float _hp;
    bool _dead;

    AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Hit(float damage)
    {
        _hp -= damage;

        if(_hp < 0 && !_dead)
        {
            StartCoroutine(DeadDelay());
            _dead = true;
        }
    }

    IEnumerator DeadDelay()
    {
        _audioSource.Play();

        GetComponent<Movement>().enabled= false;
        //GetComponentInChildren<Grabbable>()._holding = false;
        //GetComponentInChildren<Grabbable>().enabled = false;

        yield return new WaitForSeconds(3);

        int _scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(_scene);
    }

}
