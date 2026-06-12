using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public float range = 1;

    public Transform centrePoint;

    public bool isWaitIngForCoroutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        centrePoint = transform;
    }

    
    void Update()
    {
        if(agent.remainingDistance <= agent.stoppingDistance && !isWaitIngForCoroutine)
        {
            StartCoroutine(DelayNewPath());
        }

    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, range, NavMesh.AllAreas))
        { 
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    IEnumerator DelayNewPath()
    {
        isWaitIngForCoroutine = true;

        yield return new WaitForSeconds(1);

        Debug.Log("Kukas searching!");

        Vector3 point;

        if (RandomPoint(centrePoint.position, range, out point))
        {
            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
            agent.SetDestination(point);
            Debug.Log("Kukas moving!");
        }

        isWaitIngForCoroutine = false;
    }
    
}

