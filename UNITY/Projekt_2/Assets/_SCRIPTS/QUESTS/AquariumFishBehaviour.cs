using Unity.VisualScripting;
using UnityEngine;

public class AquariumFishBehaviour : MonoBehaviour
{
    [SerializeField] GameObject _fish;
    [SerializeField] Vector3 _aquariumCenter;
    [SerializeField] float _targetFishSpeed;
    [SerializeField] float _fishSpeedMargin;
    Vector3 _fishTargetPosition;
    Vector3 _scannerPointerPosition;
    float _previousAngle;
    float _fishSpeed;
    float _timer = 3;
    bool _isMinigameRunning;
    

    void Update()
    {
        if (_isMinigameRunning)
        {
            _scannerPointerPosition = ScannerController.instance.PointerWorldPosition;
            _fishTargetPosition = new Vector3(_scannerPointerPosition.x, _fish.transform.position.y, _scannerPointerPosition.z);
            GetAngleDelta();
            FishFollowPointer();
            CountEnergyTime();
        }
    }
    void GetAngleDelta()
    {
        float angle = Mathf.Atan2(_scannerPointerPosition.y, _scannerPointerPosition.x);
        float angleDelta = angle - _previousAngle;
        _fishSpeed = angleDelta * 10f;
    }


    void FishFollowPointer()
    {
        _fish.transform.position = Vector3.MoveTowards(_fish.transform.position, _fishTargetPosition, _fishSpeed);
    }
    
    void CountEnergyTime()
    {
        if(_fishSpeed >= _targetFishSpeed - _fishSpeedMargin && _fishSpeed <= _targetFishSpeed + _fishSpeedMargin)
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            _timer = 3;
        }

        if(_timer <= 0)
        {
            _isMinigameRunning = false;
        }
    }  
}
