using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

enum MaterialType
{
    GREEN,
    RED
}

public class CalibrationGameManager : MonoBehaviour
{
    [SerializeField] GameObject _targetPrefab;
    [SerializeField] GameObject _armPrefab;
    [SerializeField] Material[] _armMaterials = new Material[2];
    [SerializeField] Player_Controller _playerController;
    [SerializeField] GameObject _uiContentParent;
    [SerializeField] Image _matchFillBar;
    [SerializeField] Image _sensitivityBar;


    GameObject _target;
    GameObject _playerArm;
    GameObject _targetArm;

    Animator _playerArmAnimator;
    Animator _targetAnimator;

    Vector3 _targetPosition = new Vector3(0,0,2);
    Vector3 _pointerPosition;

    MaterialType _currentMaterial;

    int _minigameStage;

    float _angle = 0;
    float _positionMatch = 0f;

    bool _moveTarget;
    bool _allowEquipMethod;
    bool _IsArmEquipped;
    bool _minigameRunning;

    void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onEquipScanner += EquipArm;
        GameEventsManager.instance.inputEvents.onEquipScanner += ChangeMouseSensitivity;
    }

    void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onEquipScanner -= EquipArm;
        GameEventsManager.instance.inputEvents.onEquipScanner -= ChangeMouseSensitivity;
    }

    void Start()
    {
        _matchFillBar.fillAmount = 0;
        _sensitivityBar.fillAmount = 0;
        _uiContentParent.SetActive(false);

        StartMinigame();
    }

    // Update is called once per frame
    void Update()
    {
        if (_moveTarget)
        {
            MoveTargetInCircle();
        }

        if (_minigameRunning)
        {
            _targetPosition = _target.transform.position;

            ScreenToWorldPoint();
            RotateArms();

            ComparePointerPosition();
        }
    }

    void StartMinigame()
    {
        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.CALIBRATING);

        GameEventsManager.instance.playerEvents.TogglePlayerCamera(false);
        GameEventsManager.instance.playerEvents.TogglePlayerMovement(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _target = Instantiate(_targetPrefab, Camera.main.transform);
        _target.transform.localPosition = new Vector3(0, 0, 2);

        _targetAnimator = _target.GetComponent<Animator>();

        _allowEquipMethod = true;

        _uiContentParent.SetActive(true);
    }

    void EndMinigame()
    {
        _moveTarget = false;
        _minigameRunning = false;

        Destroy(_target);
        Destroy(_targetArm);

        _allowEquipMethod = true;
    }

    void ComparePointerPosition()
    {
        Vector3 difference = _targetPosition - _pointerPosition;

        if(difference.magnitude <= 0.2f)
        {
            _positionMatch = Mathf.Clamp(_positionMatch += 25f * Time.deltaTime, 0, 100);

            if(_currentMaterial != MaterialType.GREEN)
            {
                ChangeTargetArmMaterial(MaterialType.GREEN);
            }
        }
        else
        {
            _positionMatch = Mathf.Clamp(_positionMatch -= 15f * Time.deltaTime, 0, 100);

            if(_currentMaterial != MaterialType.RED)
            {
                ChangeTargetArmMaterial(MaterialType.RED);
            }
        }

        _matchFillBar.fillAmount = _positionMatch / 100f;

        if(_positionMatch >= 100f)
        {
            _minigameStage++;

            ChangeTargetState(_minigameStage);
        }
    }

    void EquipArm(InputEventContext context, float value)
    {
        if(context != InputEventContext.CALIBRATING || !_allowEquipMethod) return;

        switch (value)
        {
            case 1f:
                if(_IsArmEquipped) return;

                _IsArmEquipped = true;

                Vector3 armPosition = new Vector3(0.31f, -0.19f,-0.024f);
                _playerArm = Instantiate(_armPrefab, Camera.main.transform);
                _targetArm = Instantiate(_armPrefab, Camera.main.transform);
                
                _playerArm.transform.localPosition = armPosition;
                _targetArm.transform.localPosition = armPosition;

                ChangeTargetArmMaterial(MaterialType.RED);

                _playerArmAnimator = _targetArm.GetComponent<Animator>();

                StartCoroutine(EquipArmAnim(true));

                _allowEquipMethod = false;

                break;
            case -1f:
            if(!_IsArmEquipped) return;

                _IsArmEquipped = false;

                _allowEquipMethod = false;

                StartCoroutine(EquipArmAnim(false));
                break;
        }
    }

    void ChangeTargetState(int stage)
    {
        switch (stage)
        {
            case 1:
                _targetAnimator.SetInteger("Stage", 1);
                _positionMatch = 0;
                break;
            case 2:
                _targetAnimator.SetInteger("Stage", 2);
                _positionMatch = 0;
                break;
            case 3:
                _targetAnimator.SetInteger("Stage", 3);
                _positionMatch = 0;
                break;
            case 4:
                _targetAnimator.SetInteger("Stage", 4);
                _positionMatch = 0;
                _moveTarget = true;
                break; 
            case 5:
                _moveTarget = false;
                EndMinigame();
                break;
        }
    }

    void MoveTargetInCircle()
    {
        _angle += Time.deltaTime;

        _targetPosition.x = 0.75f * Mathf.Cos(-_angle);
        _targetPosition.y = 0.75f * Mathf.Sin(-_angle);
    }

    void ScreenToWorldPoint()
    {
        //Mouse.current.position.ReadValue();
        Vector2 cursorPosition = CursorController.instance.Cursor.transform.position;

        Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(cursorPosition.x, cursorPosition.y, 2));

        _pointerPosition = point;
    }

    void RotateArms()
    {
        _playerArm.transform.LookAt(_pointerPosition);
        _targetArm.transform.LookAt(_targetPosition);
    }

    void ChangeMouseSensitivity(InputEventContext context, float value)
    {
        if(context != InputEventContext.CALIBRATING || !_minigameRunning) return;

        _playerController.AddLookSensitivity(value);

        _sensitivityBar.fillAmount = _playerController.LookSensitivity / 10f;
    }

    void ChangeTargetArmMaterial(MaterialType type)
    {
        switch (type)
        {
            case MaterialType.GREEN:
                _targetArm.GetComponentInChildren<Renderer>().material = _armMaterials[0];
                _currentMaterial = MaterialType.GREEN;
                break;
            case MaterialType.RED:
                _targetArm.GetComponentInChildren<Renderer>().material = _armMaterials[1];
                _currentMaterial = MaterialType.RED;
                break;
        }
    }

    IEnumerator EquipArmAnim(bool toggle)
    {
        switch (toggle)
        {
            case true:
                _playerArmAnimator.SetBool("IsEquipped", true);

                yield return new WaitForSeconds(0.15f);

                _minigameRunning = true;

                ChangeTargetState(1);

                ChangeMouseSensitivity(InputEventContext.CALIBRATING, 0);
                _minigameStage = 1;

                break;
            case false:
                _minigameRunning = false;

                _playerArmAnimator.SetBool("IsEquipped", false);

                yield return new WaitForSeconds(0.15f);

                _playerArmAnimator = null;

                Destroy(_playerArm);

                GameEventsManager.instance.playerEvents.TogglePlayerCamera(true);
                GameEventsManager.instance.playerEvents.TogglePlayerMovement(true);

                Cursor.lockState = CursorLockMode.Locked;

                GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DEFAULT);
                break;
        }
    }
}
