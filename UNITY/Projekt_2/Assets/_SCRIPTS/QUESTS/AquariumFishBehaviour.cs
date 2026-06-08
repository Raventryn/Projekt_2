using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AquariumFishBehaviour : MonoBehaviour
{
    [SerializeField] GameObject _fish;
    [SerializeField] GameObject _aquarium;
    [SerializeField] BoxCollider _aquariumBorders;
    [SerializeField] float _targetFishSpeed;
    [SerializeField] float _fishSpeedMargin;
    [SerializeField] float _baseFishSpeed = 0.5f;
    [SerializeField] FishPatrolBehaviour _patrolBehaviour;
    [SerializeField] TMP_Text _powerText;
    [SerializeField] Image _fillBar;
    Vector3 _aquariumCenter;
    Vector3 _fishTargetPosition;
    Vector3 _scannerPointerPosition;
    Vector3 _currentFishTarget;

    List<float> deltas = new List<float>();
    List<Vector3> fishTargets = new List<Vector3>();

    float _fullTimerValue;
    float _previousAngle;
    float _fishSpeed;
    float _timer = 3;
    public bool _isMinigameRunning;
    bool isInvoking;

    void OnEnable()
    {
        GameEventsManager.instance.questEvents.onStartFishMinigame += StartOrStopMinigame;
    }

    void OnDisable()
    {
        GameEventsManager.instance.questEvents.onStartFishMinigame -= StartOrStopMinigame;
    }
    
    void Start()
    {
        _aquariumCenter = _aquarium.transform.position + new Vector3(0, _aquariumBorders.size.y / 2f, 0);
        _fullTimerValue = _timer;

        _fillBar.fillAmount = 0;
        _powerText.text = "";
    }

    void Update()
    {
        if (_isMinigameRunning && ScannerController.instance.IsScanning)
        {
            if (_patrolBehaviour.IsPatroling)
                _patrolBehaviour.IsPatroling = false;
            

            _scannerPointerPosition = ScannerController.instance.PointerWorldPosition;
            Vector3 _localPointerPosition = _aquarium.transform.InverseTransformPoint(_scannerPointerPosition);
            _fishTargetPosition = new Vector3(_localPointerPosition.x, _aquariumCenter.y, _localPointerPosition.z);
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
            if(!_patrolBehaviour.IsPatroling)
                _patrolBehaviour.IsPatroling = true;
        }
    }

    public void StartOrStopMinigame(bool toggle)
    {
        if (toggle)
        {
            _isMinigameRunning = true;
        }
        else
        {
            _patrolBehaviour.IsPatroling = true;
            _isMinigameRunning = false;
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

        if(fishTargets.Count > 3)
        {
            fishTargets.RemoveAt(0);
        }

        _currentFishTarget = fishTargets[0];
    }

    void FishFollowPointer()
    {
        Vector3 newPosition = Vector3.MoveTowards(_fish.transform.localPosition, _currentFishTarget, _fishSpeed * Time.deltaTime);
        newPosition.x = Mathf.Clamp(newPosition.x, - _aquariumBorders.size.x/2 + _patrolBehaviour.PositionPadding, _aquariumBorders.size.x/2 - _patrolBehaviour.PositionPadding);
        newPosition.z = Mathf.Clamp(newPosition.z, - _aquariumBorders.size.z/2 + _patrolBehaviour.PositionPadding, _aquariumBorders.size.z/2 - _patrolBehaviour.PositionPadding);

        _fish.transform.localPosition = newPosition;
        
        if((_currentFishTarget - _fish.transform.localPosition).magnitude >= Mathf.Epsilon)
        {
             _fish.transform.rotation = Quaternion.Slerp(_fish.transform.rotation, Quaternion.LookRotation(_currentFishTarget - _fish.transform.localPosition, Vector3.up), 20 * Time.deltaTime);
        }
            
        //Debug.Log(_fishSpeed);
    }
    
    void CountEnergyTime()
    {
        if(_fishSpeed >= _targetFishSpeed - _fishSpeedMargin && _fishSpeed <= _targetFishSpeed + _fishSpeedMargin)
        {
            _timer = Mathf.Clamp(_timer - Time.deltaTime, 0, _fullTimerValue);
        }
        else
        {
            _timer = Mathf.Clamp(_timer + Time.deltaTime, 0, _fullTimerValue);
        }

        _fillBar.fillAmount = (_fullTimerValue - _timer) / 3f;

        _powerText.text = (_fishSpeed * 2.175f).ToString("F1") + "KW";

        if(_timer <= 0)
        {
            _isMinigameRunning = false;
            _patrolBehaviour.IsPatroling = true;
        }
    }  
}
