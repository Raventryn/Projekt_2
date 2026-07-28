using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

enum LineColors
{
    GREEN,
    YELLOW,
    ORANGE,
    RED
}

public class CompareWaves : MonoBehaviour
{
    [SerializeField] GameObject minigameUIContentParent;
    [SerializeField] Animator contentParentAnimator;

    [SerializeField] PlayerSinewave _playerWave;
    [SerializeField] Sinewave _targetWave;
    [SerializeField] LineRenderer _progressGraph;
    [SerializeField] LineRenderer _transferSpeedBar;
    [SerializeField] LineRenderer _progressBar;

    [SerializeField] TMP_Text _transferSpeedText;

    [SerializeField] Button startMinigameButton;

    [SerializeField] Vector2 _progressGraphLimits = new Vector2(0, 10);

    [SerializeField] Color[] lineColors;

    AudioSource audioSource;
    LineColors previousColor;
    ScannableObjectType objectType;
    Vector3 _playerLinePosition;


//Only for testing, remove if not needed anymore
    [SerializeField] float _frequencyDifference;
    [SerializeField] float _amplitudeDifference;

    float timer = 15;
    float transferSpeed = 1;

    bool runComparison = true;

    void OnEnable()
    {
        GameEventsManager.instance.questEvents.onStartScanMinigame += ShowMinigameUI;
        GameEventsManager.instance.inputEvents.onPressedEscape += CloseMinigameUI;
    }
    void OnDisable()
    {
        GameEventsManager.instance.questEvents.onStartScanMinigame -= ShowMinigameUI;
        GameEventsManager.instance.inputEvents.onPressedEscape -= CloseMinigameUI;
    }


    void Start()
    {
        _playerLinePosition = _playerWave._lineRenderer.transform.localPosition;

        audioSource = GetComponent<AudioSource>();

        runComparison = false;

        _playerWave.WaveSpeed = 0;
        _targetWave.WaveSpeed = 0;

        ChangeWaveColor(LineColors.GREEN);

        minigameUIContentParent.SetActive(false);

        //Remove when event is implemented
        //ShowMinigameUI(true);
    }

    void Update()
    {
        if (runComparison)
        {
            AddProgressBarPosition();
            CompareWaveValues();
        } 
    }

    void ShowMinigameUI(bool toggle, ScannableObjectType type)
    {
        objectType = type;
        StartCoroutine(ToggleContentParent(toggle));

        if(type == ScannableObjectType.GAMESTART_DUMMY)
            GameEventsManager.instance.soundEvents.ChangeAudioVolume(0.25f);
    }

    void CloseMinigameUI(InputEventContext context)
    {
        if(context != InputEventContext.SCANNER_MINIGAME || objectType == ScannableObjectType.GAMESTART_DUMMY) return;

        StartCoroutine(ToggleContentParent(false));
    }

    public void StartMinigame()
    {
        startMinigameButton.gameObject.SetActive(false);

        runComparison = true;

        _playerWave.WaveSpeed = 10;
        _targetWave.WaveSpeed = 10;

        _playerWave.Amplitude = _targetWave.Amplitude;
        _playerWave.Frequency = _targetWave.Frequency;

        GameEventsManager.instance.inputEvents.ShowCursor(false);

        InvokeRepeating("SimplifyProgressGraph", 2, 2);

        audioSource.volume = 0;
        audioSource.Play();
    }

    void MatchAudioVolume(float value)
    {
        audioSource.volume = Mathf.Clamp(value , 0, 0.75f);
    }

    void ChangeAudioPitch(float value)
    {
        audioSource.pitch = Mathf.Clamp(value , 0.7f, 1.3f);
    }

    public void ResetMinigame()
    {
        _progressGraph.positionCount = 0;
        _progressGraph.SetPositions(new Vector3[0]);
        timer = 15;
        transferSpeed = 1;

        startMinigameButton.gameObject.SetActive(true);
    }

    IEnumerator ToggleContentParent(bool isActive)
    {
        GameEventsManager.instance.uiEvents.GlitchOnMenu();

        switch (isActive)
        {
            case true:
                minigameUIContentParent.SetActive(true);
                contentParentAnimator.SetBool("IsActive", true);

                yield return new WaitForSeconds(0.15f);

                GameEventsManager.instance.playerEvents.BlockPlayerInput(true);

                GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.SCANNER_MINIGAME);

                GameEventsManager.instance.playerEvents.TogglePlayerCamera(false);
                GameEventsManager.instance.playerEvents.TogglePlayerMovement(false);
                GameEventsManager.instance.inputEvents.ShowCursor(true);

                startMinigameButton.gameObject.SetActive(true);
                break;
            case false:
                contentParentAnimator.SetBool("IsActive", false);
                startMinigameButton.gameObject.SetActive(false);

                audioSource.Stop();

                yield return new WaitForSeconds(0.15f);

                minigameUIContentParent.SetActive(false);

                if (!ScannerController.instance.IsInScanView)
                {
                    GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DEFAULT);
                }
                else
                {
                    GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.SCANNER_VIEW);
                }

                GameEventsManager.instance.questEvents.DisableGlitches();
                
                GameEventsManager.instance.playerEvents.BlockPlayerInput(false);

                GameEventsManager.instance.playerEvents.TogglePlayerCamera(true);
                GameEventsManager.instance.playerEvents.TogglePlayerMovement(true);
                GameEventsManager.instance.inputEvents.ShowCursor(false);
                break;
        }
    }

    void CompareWaveValues()
    {
        float amplitudeDifference = Mathf.Sqrt(Mathf.Pow(_targetWave.Amplitude - _playerWave.Amplitude, 2));
        float frequencyDifference = Mathf.Sqrt(Mathf.Pow(_targetWave.Frequency - _playerWave.Frequency, 2));

        _frequencyDifference = frequencyDifference;
        _amplitudeDifference = amplitudeDifference;

        float totalDifference = amplitudeDifference + frequencyDifference;

        //Debug.Log(transferSpeed);

        for(int i = 0; i < 2; i++)
        {
            Vector3 pointPosition = _transferSpeedBar.GetPosition(i);
            Vector3 newPosition = new Vector3(pointPosition.x, transferSpeed * 5, pointPosition.z);
            _transferSpeedBar.SetPosition(i, newPosition);
        }

        MatchAudioVolume(1-transferSpeed);
        ChangeAudioPitch(0.7f + (totalDifference * 0.2f));

        if(amplitudeDifference <= 0.21f && frequencyDifference <= 0.023f)
        {

            timer -= 1 * transferSpeed * Time.deltaTime;

                if(transferSpeed < 1)
                {
                    transferSpeed = Mathf.Clamp(transferSpeed += 0.1f * Time.deltaTime, 0, 1);
                }

            if(previousColor != LineColors.GREEN)
            {
                ChangeWaveColor(LineColors.GREEN);
                ResetWavePositition();
            }
        }
        else if(amplitudeDifference <= 0.29f && frequencyDifference <= 0.035f)
        {
            timer -= 1 * transferSpeed * Time.deltaTime;

            if(previousColor != LineColors.YELLOW)
            {
                ChangeWaveColor(LineColors.YELLOW);
                ResetWavePositition();
            }
        }
        else
        {

            transferSpeed = Mathf.Clamp(transferSpeed -= 0.088f * Time.deltaTime, 0, 1);

            ShakeWave(totalDifference);

            if(transferSpeed > 0.15f)
            {
                if(previousColor != LineColors.ORANGE)
                {
                    ChangeWaveColor(LineColors.ORANGE);
                }
            }
            else
            {
                if(previousColor != LineColors.RED)
                {
                    ChangeWaveColor(LineColors.RED);
                }
            }

            

            if(transferSpeed <= 0)
            {
                runComparison = false;

                _playerWave.WaveSpeed = 0;
                _targetWave.WaveSpeed = 0;

                Debug.Log("Failed!");

                audioSource.Stop();

                GameEventsManager.instance.inputEvents.ShowCursor(true);
                //restartMinigameButton.gameObject.SetActive(true);
                ResetMinigame();
                return;
            } 
        }

        for(int i = 0; i < 2; i++)
        {
            Vector3 pointPosition = _progressBar.GetPosition(i);
            Vector3 newPosition = new Vector3((1 - (timer / 15f)) * _progressGraphLimits.y, pointPosition.y, pointPosition.z);
            _progressBar.SetPosition(i, newPosition);
        }

        string transferSpeedText = transferSpeed.ToString("F2");

        _transferSpeedText.text = "Transfer speed: " + Mathf.CeilToInt(transferSpeed * 15000f) * 0.001f +" Tbps";

        if(timer <= 0)
        {
            runComparison = false;

            _playerWave.WaveSpeed = 0;
            _targetWave.WaveSpeed = 0;
            
            Debug.Log("Succeeded!");

            audioSource.Stop();

            ShowMinigameUI(false, objectType);
            ResetMinigame();
            if(objectType != ScannableObjectType.GAMESTART_DUMMY)
            {
                ScannerManager.instance.ScannedObjects[objectType] = true;
                GameEventsManager.instance.interactionEvents.UpdateObjectScannedState(objectType);
                ExperienceManager.instance.AddMoney(Random.Range(12, 25));
            }

            //GameEventsManager.instance.soundEvents.TriggerSound(SoundType.GLITCH, false);

            GameEventsManager.instance.questEvents.FinishedScanMinigame();
            GameEventsManager.instance.questEvents.HideScanGlitch(objectType);
            //Send Event that minigame is finished
            //If object is to be picked up, change tag and layer of object
        }
    } 

    void AddProgressBarPosition()
    {
        _progressGraph.positionCount += 1;

        int lastPosition = _progressGraph.positionCount - 1;

        Vector3 position = new Vector3((1 - (timer / 15f)) * _progressGraphLimits.y, transferSpeed * 5, 0);
        
        _progressGraph.SetPosition(lastPosition, position);
    }

    void SimplifyProgressGraph()
    {
        _progressGraph.Simplify(0.025f);
    }

    void ChangeWaveColor(LineColors newColor)
    {
        switch (newColor)
        {
            case LineColors.GREEN:
                _playerWave._lineRenderer.startColor = lineColors[0];
                _playerWave._lineRenderer.endColor = lineColors[0];
                break;
            case LineColors.YELLOW:
                _playerWave._lineRenderer.startColor = lineColors[1];
                _playerWave._lineRenderer.endColor = lineColors[1];
                break;
            case LineColors.ORANGE:
                _playerWave._lineRenderer.startColor = lineColors[2];
                _playerWave._lineRenderer.endColor = lineColors[2];
                break;
            case LineColors.RED:
                _playerWave._lineRenderer.startColor = lineColors[3];
                _playerWave._lineRenderer.endColor = lineColors[3];
                break;
        }

        previousColor = newColor;
    }

    void ShakeWave(float totalDifference)
    {
        float shakeStrength = 2 * (totalDifference / 1);

        float Tau = 2 * Mathf.PI;

        float x = Time.timeSinceLevelLoad * 10f;
        float positionY = shakeStrength * Mathf.Sin(x * Tau * totalDifference);
        float positionX = shakeStrength * Mathf.Cos(x * Tau * totalDifference);

        Vector3 newPosition = new Vector3(_playerLinePosition.x + positionX, _playerLinePosition.y + positionY, 0);

        _playerWave._lineRenderer.transform.localPosition = newPosition;
    }

    void ResetWavePositition()
    {
        _playerWave._lineRenderer.transform.localPosition = _playerLinePosition;
    }
}
