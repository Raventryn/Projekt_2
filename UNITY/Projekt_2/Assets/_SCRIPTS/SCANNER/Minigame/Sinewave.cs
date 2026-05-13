using UnityEngine;

[ExecuteInEditMode]
public class Sinewave : MonoBehaviour
{
    [SerializeField] bool _isPlayerControlled;
    [SerializeField] bool _isRandomizeWave;

    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] int _points;
    [SerializeField] Vector2 xLimits = new Vector2(0, 1);

    float _newFrequency;
    float _newAmplitude;
    bool _IsMoveWaveValues;

    public float Amplitude = 1;
    public float Frequency = 1;

    public float WaveSpeed = 1;

    void OnEnable()
    {
        if(_isPlayerControlled) GameEventsManager.instance.inputEvents.onLook += ManipulateWave;
    }

    void OnDisable()
    {
        if(_isPlayerControlled) GameEventsManager.instance.inputEvents.onLook -= ManipulateWave;
    }

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        if(_isRandomizeWave) InvokeRepeating("RandomizeWave", 0, 8);
    }

    void Update()
    {
        if (_IsMoveWaveValues && _isRandomizeWave && WaveSpeed != 0)
        {
            MoveToRandomValues();
        }

        Draw();
    }

    void Draw()
    {
        float xStart = xLimits.x;
        float Tau = 2 * Mathf.PI;
        float xFinish = xLimits.y;

        _lineRenderer.positionCount = _points;
        for(int currentPoint = 0; currentPoint < _points; currentPoint++)
        {
            float progress = (float)currentPoint/(_points-1);
            float x = Mathf.Lerp(xStart, xFinish, progress);
            float y = Amplitude * Mathf.Sin((x * Tau * Frequency) + Time.timeSinceLevelLoad * WaveSpeed);

            _lineRenderer.SetPosition(currentPoint, new Vector3(x,y,0));
        }
    }

    void ManipulateWave(InputEventContext context, Vector2 value)
    {
        if(/*context != InputEventContext.SCANNER_MINIGAME && */ WaveSpeed == 0) return;

        Frequency = Mathf.Clamp(Frequency += -value.x * 0.001f, 0.33f, 1.25f);
        Amplitude = Mathf.Clamp(Amplitude += value.y * 0.001f, 0.33f, 1.5f);
    }

    void RandomizeWave()
    {

        _newFrequency = Random.Range(0.33f, 1.25f);

        _newAmplitude = Random.Range(0.33f, 1.5f);

        _IsMoveWaveValues = true;
    }

    void MoveToRandomValues()
    {
        Frequency = Mathf.MoveTowards(Frequency, _newFrequency, 0.15f * Time.deltaTime);
        Amplitude = Mathf.MoveTowards(Amplitude, _newAmplitude, 0.15f * Time.deltaTime);

        if(Frequency == _newFrequency && Amplitude == _newAmplitude)
        {
            _IsMoveWaveValues = false;
        }
    }
}
