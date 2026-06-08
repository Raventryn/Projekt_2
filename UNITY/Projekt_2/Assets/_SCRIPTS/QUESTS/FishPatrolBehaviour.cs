using UnityEngine;

public class FishPatrolBehaviour : MonoBehaviour
{
    [SerializeField] BoxCollider _aquariumCollider;
    [SerializeField] GameObject _fish;
    [SerializeField] float _fishSpeed = 1;
    public float PositionPadding = 0;
    Vector3 _currentTarget;

    public bool IsPatroling = true;

    void Start()
    {
        InvokeRepeating("GenerateTargetPoint", 0, 2);
    }

    void Update()
    {
        if (IsPatroling)
        {
            MoveToTarget();
        }
    }
    void GenerateTargetPoint()
    {
        float posX = Random.Range(_aquariumCollider.center.x - _aquariumCollider.size.x/2 + PositionPadding, _aquariumCollider.center.x + _aquariumCollider.size.x/2 - PositionPadding);
        float posY = Random.Range(_aquariumCollider.center.y - _aquariumCollider.size.y/2 + PositionPadding, _aquariumCollider.center.y + _aquariumCollider.size.y/2 - PositionPadding);
        float posZ = Random.Range(_aquariumCollider.center.z - _aquariumCollider.size.z/2 + PositionPadding, _aquariumCollider.center.z + _aquariumCollider.size.z/2 - PositionPadding);

        Vector3 target = new Vector3(posX, posY, posZ);

        _currentTarget = target;
    }

    void MoveToTarget()
    {
        Vector3 newPosition = Vector3.MoveTowards(_fish.transform.localPosition, _currentTarget, _fishSpeed * Time.deltaTime);
        _fish.transform.localPosition = newPosition;

        if((_currentTarget - _fish.transform.localPosition).magnitude >= 0.0005f)
            _fish.transform.rotation = Quaternion.Slerp(_fish.transform.rotation, Quaternion.LookRotation(_currentTarget - _fish.transform.localPosition, Vector3.up), 20 * Time.deltaTime);
    }
}
