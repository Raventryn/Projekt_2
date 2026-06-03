using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class AquariumFishBehaviour : MonoBehaviour
{
    [SerializeField] GameObject targetVisualizer;

    [SerializeField] GameObject _fish;
    [SerializeField] GameObject _aquarium;
    [SerializeField] float _targetFishSpeed;
    [SerializeField] float _fishSpeedMargin;
    [SerializeField] float _baseFishSpeed = 0.5f;
    Vector3 _aquariumCenter;
    Vector3 _fishTargetPosition;
    Vector3 _scannerPointerPosition;
    Vector3 _currentFishTarget;

    List<float> deltas = new List<float>();
    List<Vector3> fishTargets = new List<Vector3>();

    float _previousAngle;
    float _fishSpeed;
    float _timer = 3;
    public bool _isMinigameRunning;
    bool isInvoking;
    
    void Start()
    {
        _aquariumCenter = _aquarium.transform.position;
    }

    void Update()
    {
        if (_isMinigameRunning && ScannerController.instance.IsScanning)
        {
            _scannerPointerPosition = ScannerController.instance.PointerWorldPosition;
            Vector3 _localPointerPosition = _aquarium.transform.InverseTransformPoint(_scannerPointerPosition);
            _fishTargetPosition = new Vector3(_localPointerPosition.x, _fish.transform.localPosition.y, _localPointerPosition.z);
            GetPositiveAngleDelta();
            FishFollowPointer();
            CountEnergyTime();

            if (!isInvoking)
            {
                _currentFishTarget = _fishTargetPosition * 0.1f;
                InvokeRepeating("RelativeDelta", 0.25f, 0.25f);
                InvokeRepeating("AddOrRemoveFishTarget", 0.1f, 0.1f);
                isInvoking = true;
            }
        }
        else if(!ScannerController.instance.IsScanning && isInvoking)
        {
            CancelInvoke();
            isInvoking = false;
        }
    }
    void GetPositiveAngleDelta()
    {
        float angle = Mathf.Abs(GetPointerAngle());
        float angleDelta = Mathf.Abs(angle - _previousAngle);
        _previousAngle = angle;
        deltas.Add(angleDelta * (1 - Time.deltaTime));
        //Debug.Log(deltas.Count);
    }

    void RelativeDelta()
    {
        float totalDelta = 0;

        foreach(float delta in deltas)
        {
            totalDelta += delta;
        }

        if(totalDelta != 0)
        totalDelta = totalDelta / deltas.Count;

        _fishSpeed = _baseFishSpeed + (totalDelta * 0.1f);

        deltas.Clear();
    }

    float GetPointerAngle()
    {
        Vector3 zeroAngle = (_aquariumCenter + new Vector3(1,0,0)) - _aquariumCenter;
        Vector3 pointerDirection = _aquariumCenter - new Vector3(_scannerPointerPosition.x, _aquariumCenter.y, _scannerPointerPosition.z);

        float angle = Vector3.SignedAngle(zeroAngle, pointerDirection, Vector3.up);

        return angle;
    }

    void AddOrRemoveFishTarget()
    {
        fishTargets.Add(_fishTargetPosition * 0.1f);

        Debug.Log(fishTargets.Count);

        if(_fish.transform.localPosition == _currentFishTarget)
        {
            Debug.Log("removed");
            fishTargets.RemoveAt(0);
        }

        if(_currentFishTarget != fishTargets[0])
            _currentFishTarget = fishTargets[0];
    }

    void FishFollowPointer()
    {
        targetVisualizer.transform.localPosition = _fishTargetPosition * 0.1f;
        Vector3 newPosition = Vector3.MoveTowards(_fish.transform.localPosition, _currentFishTarget, _fishSpeed * Time.deltaTime);
        _fish.transform.localPosition = newPosition;
        
        if((_currentFishTarget - _fish.transform.localPosition).magnitude >= 0.0005f)
            _fish.transform.rotation = Quaternion.Slerp(_fish.transform.rotation, Quaternion.LookRotation(_currentFishTarget - _fish.transform.localPosition, Vector3.up), 5 * Time.deltaTime);
        //Debug.Log(_fishSpeed);
    }
    
    void CountEnergyTime()
    {
        if(_fishSpeed >= _targetFishSpeed - _fishSpeedMargin && _fishSpeed <= _targetFishSpeed + _fishSpeedMargin)
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            _timer += Time.deltaTime;
        }

        if(_timer <= 0)
        {
            _isMinigameRunning = false;
        }
    }  
}
