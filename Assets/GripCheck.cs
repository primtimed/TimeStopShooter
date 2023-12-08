using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripCheck : MonoBehaviour
{
    Renderer _hand;

    private void Start()
    {
        _hand = GetComponent<Renderer>();
    }
}
