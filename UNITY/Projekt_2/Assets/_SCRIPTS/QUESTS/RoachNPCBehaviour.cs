using System.IO;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class RoachNPCBehaviour : MonoBehaviour
{
    NavMeshAgent _agent;
    RoachScanBehaviour _scanBehaviour;
    NavMeshSurface _navMesh;
    BoxCollider _walkableArea;

    bool _IsWalking;
    float _defaultSpeed;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _scanBehaviour = GetComponent<RoachScanBehaviour>();

        _navMesh = FindAnyObjectByType<NavMeshSurface>();
        _walkableArea = _navMesh.GetComponent<BoxCollider>();

        _defaultSpeed = _agent.speed;

        InvokeRepeating("SetDestination", 0, 3);
    }

    void Update()
    {
        if (_IsWalking)
        {
            if(_agent.remainingDistance <= 0.05f && !_scanBehaviour.IsBeingScanned)
            {
                Invoke("SetDestination", 1.5f);
                _IsWalking = false;
            }

        }

        if (_scanBehaviour.IsBeingScanned)
        {
            _agent.speed = _defaultSpeed * 1.2f;
        }
        else if (!_scanBehaviour.IsBeingScanned)
        {
            _agent.speed = _defaultSpeed;
        }
    }

    void SetDestination()
    {
        NavMeshPath path = new NavMeshPath();

        //bool isPathValid = false;

        Vector3 destination = GetRandomNavMeshPosition();

        _agent.CalculatePath(destination, path);

        /*while (!isPathValid)
        {
            

            if((destination - transform.position).magnitude >= 0.3f)
            {
                isPathValid = true;
            }
        }*/

        _agent.SetPath(path);

        _IsWalking = true;
    }

    public void StopBehaviour()
    {
        Destroy(this);
    }

    Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomPosition = _walkableArea.transform.position + new Vector3(Random.Range(_navMesh.center.x - _navMesh.size.x/2, _navMesh.center.x + _navMesh.size.x / 2), 0, Random.Range(_navMesh.center.z - _navMesh.size.z /2, _navMesh.center.z + _navMesh.size.z / 2));
        NavMeshHit hit;

        NavMesh.SamplePosition(randomPosition, out hit, 6f, 1 << NavMesh.GetAreaFromName("Walkable"));

        return hit.position;
    }

}
