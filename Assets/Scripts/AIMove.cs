using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AIMove : MonoBehaviour
{
    NavMeshAgent _agent;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        Vector3 _loc = new Vector3(Random.Range(-30, 10), 0, Random.Range(15, -3));

        _agent.Move(_loc);

        yield return new WaitForSeconds(5);
    }
}
