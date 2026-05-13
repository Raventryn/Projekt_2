using UnityEngine;

public class PlayerSinewave : MonoBehaviour
{
    public LineRenderer _lineRenderer;
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
        GameEventsManager.instance.inputEvents.onLook += ManipulateWave;
    }

    void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onLook -= ManipulateWave;
    }

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
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
}
