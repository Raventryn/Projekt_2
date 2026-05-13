using UnityEngine;

enum LineColors
{
    GREEN,
    YELLOW,
    ORANGE,
    RED
}

public class CompareWaves : MonoBehaviour
{
    [SerializeField] PlayerSinewave _playerWave;
    [SerializeField] Sinewave _targetWave;
    [SerializeField] LineRenderer _progressGraph;
    [SerializeField] LineRenderer _transferSpeedBar;
    [SerializeField] LineRenderer _progressBar;

    [SerializeField] Vector2 _progressGraphLimits = new Vector2(0, 10);

    [SerializeField] Color[] lineColors;
    LineColors previousColor;
    LineColors lineColor;

    float timer = 15;
    float transferSpeed = 1;

    bool runComparison = true;

    void Update()
    {
        if (runComparison)
        {
            AddProgressBarPosition();
            CompareWaveValues();
        } 
    }

    void CompareWaveValues()
    {
        float amplitudeDifference = Mathf.Sqrt(Mathf.Pow(_targetWave.Amplitude - _playerWave.Amplitude, 2));
        float frequencyDifference = Mathf.Sqrt(Mathf.Pow(_targetWave.Frequency - _playerWave.Frequency, 2));

        float totalDifference = amplitudeDifference + frequencyDifference;

        Debug.Log(transferSpeed);

        for(int i = 0; i < 2; i++)
        {
            Vector3 pointPosition = _transferSpeedBar.GetPosition(i);
            Vector3 newPosition = new Vector3(pointPosition.x, transferSpeed * 5, pointPosition.z);
            _transferSpeedBar.SetPosition(i, newPosition);
        }

        if(totalDifference <= 0.08f)
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
        else if(totalDifference <= 0.15f)
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

            transferSpeed = Mathf.Clamp(transferSpeed -= 0.15f * Time.deltaTime, 0, 1);

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
                return;
            } 
        }

        for(int i = 0; i < 2; i++)
        {
            Vector3 pointPosition = _progressBar.GetPosition(i);
            Vector3 newPosition = new Vector3((1 - (timer / 15f)) * _progressGraphLimits.y, pointPosition.y, pointPosition.z);
            _progressBar.SetPosition(i, newPosition);
        }

        if(timer <= 0)
        {
            runComparison = false;

            _playerWave.WaveSpeed = 0;
            _targetWave.WaveSpeed = 0;
            
            Debug.Log("Succeeded!");
        }
    } 

    void AddProgressBarPosition()
    {
        _progressGraph.positionCount += 1;

        int lastPosition = _progressGraph.positionCount - 1;

        Vector3 position = new Vector3((1 - (timer / 15f)) * _progressGraphLimits.y, transferSpeed * 5, 0);
        
        _progressGraph.SetPosition(lastPosition, position);
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

        Vector3 newPosition = new Vector3(positionX, positionY, 0);

        _playerWave._lineRenderer.transform.localPosition = newPosition;
    }

    void ResetWavePositition()
    {
        _playerWave._lineRenderer.transform.localPosition = Vector3.zero;
    }
}
