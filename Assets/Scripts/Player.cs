using NUnit.Framework.Internal;
using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float _hp;
    public bool _dead;

    AudioSource _audioSource;

    public Cheakpoint _cheackpoint;

    private void Awake()
    {
        if (_cheackpoint._data._index != 0)
        {
            transform.position = _cheackpoint._points[_cheackpoint._data._index - 1].position;
        }
    }

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

        GetComponent<Movement>().enabled = false;

        yield return new WaitForSeconds(3);

        int _scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(_scene);
    }

}
