using UnityEngine;

[ExecuteInEditMode]
public class Sinewave : MonoBehaviour
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

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        InvokeRepeating("RandomizeWave", 0, 8);
    }

    void Update()
    {
        if (_IsMoveWaveValues  && WaveSpeed != 0)
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

    void RandomizeWave()
    {

        _newFrequency = Random.Range(0.33f, 1.1f);

        _newAmplitude = Random.Range(0.55f, 2.9f);

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
