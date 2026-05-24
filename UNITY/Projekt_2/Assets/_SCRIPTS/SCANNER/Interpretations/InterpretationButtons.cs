using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;

public class InterpretationButtons : MonoBehaviour
{
    [SerializeField] GameObject _canvasContainer;
    [SerializeField] Animator _canvasAnimator;

    [SerializeField] Button _button0;
    [SerializeField] Button _button1;
    [SerializeField] Button _button2;


    [Header("Case SPHERE")]
    [SerializeField] ScanObject _sphereOptionZERO;
    [SerializeField] ScanObject _sphereOptionONE;
    [SerializeField] ScanObject _sphereOptionTWO;

    [Header("Case CAPSULE")]
    [SerializeField] ScanObject _capsuleOptionZERO;
    [SerializeField] ScanObject _capsuleOptionONE;
    [SerializeField] ScanObject _capsuleOptionTWO;

    [Header("Case CUBE")]
    [SerializeField] ScanObject _cubeOptionZERO;
    [SerializeField] ScanObject _cubeOptionONE;
    [SerializeField] ScanObject _cubeOptionTWO;

    [Header("Case CYLINDER")]
    [SerializeField] ScanObject _cylinderOptionZERO;
    [SerializeField] ScanObject _cylinderOptionONE;
    [SerializeField] ScanObject _cylinderOptionTWO;

    TMP_Text _buttonZEROText;
    TMP_Text _buttonONEText;
    TMP_Text _buttonTWOText;

    ScannableObjectType _type;

    ScanObject _choiceZERO;
    ScanObject _choiceONE;
    ScanObject _choiceTWO;


    void OnEnable()
    {
        GameEventsManager.instance.questEvents.onShowButtonCanvas += ShowButtonCanvas;
    }

    void OnDisable()
    {
        GameEventsManager.instance.questEvents.onShowButtonCanvas -= ShowButtonCanvas;
    }

    void Start()
    {
        _buttonZEROText = _button0.GetComponentInChildren<TMP_Text>();
        _buttonONEText = _button1.GetComponentInChildren<TMP_Text>();
        _buttonTWOText = _button2.GetComponentInChildren<TMP_Text>();
    }

    void ShowButtonCanvas(bool toggle, ScannableObjectType type)
    {
        _type = type;

        switch (type)
        {
            case ScannableObjectType.SPHERE:
                _choiceZERO = _sphereOptionZERO;
                _buttonZEROText.text = _sphereOptionZERO.gameObject.name;

                _choiceONE = _sphereOptionONE;
                _buttonONEText.text = _sphereOptionONE.gameObject.name;

                _choiceTWO = _sphereOptionTWO;
                _buttonTWOText.text = _sphereOptionTWO.gameObject.name;
                break;
            case ScannableObjectType.CAPSULE:
                _choiceZERO = _capsuleOptionZERO;
                _buttonZEROText.text = _capsuleOptionZERO.gameObject.name;

                _choiceONE = _capsuleOptionONE;
                _buttonONEText.text = _capsuleOptionONE.gameObject.name;

                _choiceTWO = _capsuleOptionTWO;
                _buttonTWOText.text = _capsuleOptionTWO.gameObject.name;

                break;
            case ScannableObjectType.CUBE:
                _choiceZERO = _cubeOptionZERO;
                _buttonZEROText.text = _cubeOptionZERO.gameObject.name;

                _choiceONE = _cubeOptionONE;
                _buttonONEText.text = _cubeOptionONE.gameObject.name;

                _choiceTWO = _cubeOptionTWO;
                _buttonTWOText.text = _cubeOptionTWO.gameObject.name;

                break;
            case ScannableObjectType.CYLINDER:
                _choiceZERO = _cylinderOptionZERO;
                _buttonZEROText.text = _cylinderOptionZERO.gameObject.name;

                _choiceONE = _cylinderOptionONE;
                _buttonONEText.text = _cylinderOptionONE.gameObject.name;

                _choiceTWO = _cylinderOptionTWO;
                _buttonTWOText.text = _cylinderOptionTWO.gameObject.name;

                break;
        }

        StartCoroutine(ButtonCanvasAnimation(toggle));
    }

    public void SetChoice(int i)
    {
        switch (i)
            {
                case 0:
                    GameEventsManager.instance.questEvents.ReplaceInterpretableObjects(_type, _choiceZERO.gameObject);
                    
                    break;
                case 1:
                    GameEventsManager.instance.questEvents.ReplaceInterpretableObjects(_type, _choiceONE.gameObject);

                    break;
                case 2:
                    GameEventsManager.instance.questEvents.ReplaceInterpretableObjects(_type, _choiceTWO.gameObject);

                    break;
            }

            StartCoroutine(ButtonCanvasAnimation(false));
    }

        IEnumerator ButtonCanvasAnimation(bool toggle)
    {
        switch (toggle)
        {
            case true:
                _canvasContainer.SetActive(toggle);

                _canvasAnimator.SetBool("showButtons", toggle);

                GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.SCANNER_BUTTONS);

                yield return new WaitForSeconds(0.15f);

                GameEventsManager.instance.playerEvents.TogglePlayerCamera(false);
                GameEventsManager.instance.playerEvents.TogglePlayerMovement(false);
                GameEventsManager.instance.inputEvents.ShowCursor(true);
         
                break;

            case false:
                _canvasAnimator.SetBool("showButtons", toggle);

                yield return new WaitForSeconds(0.15f);

                _canvasContainer.SetActive(toggle);

                if (!ScannerController.instance.IsInScanView)
                {
                    GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.SCANNER);
                }
                else
                {
                    GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.SCANNER_VIEW);
                }
                

                GameEventsManager.instance.playerEvents.TogglePlayerCamera(true);
                GameEventsManager.instance.playerEvents.TogglePlayerMovement(true);
                GameEventsManager.instance.inputEvents.ShowCursor(false);
                
                break;
        }
    }
    
}
