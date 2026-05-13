using UnityEngine;

public class CompareWaves : MonoBehaviour
{
    [SerializeField] Sinewave _playerWave;
    [SerializeField] Sinewave _targetWave;
    [SerializeField] LineRenderer _progressBar;

    [SerializeField] Vector2 _progressBarLimits = new Vector2(0, 10);

    float timer = 15;
    float transferSpeed = 1;

    bool runComparison = true;
    bool wavesMatching = false;

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

        if(totalDifference <= 0.08f)
        {

            timer -= 1 * transferSpeed * Time.deltaTime;

                if(transferSpeed < 1)
                {
                    transferSpeed = Mathf.Clamp(transferSpeed += 0.1f * Time.deltaTime, 0, 1);
                }
        }
        else if(totalDifference <= 0.15f)
        {

            timer -= 1 * transferSpeed * Time.deltaTime;
        }
        else
        {

            transferSpeed = Mathf.Clamp(transferSpeed -= 0.15f * Time.deltaTime, 0, 1);

            if(transferSpeed <= 0)
            {
                runComparison = false;

                _playerWave.WaveSpeed = 0;
                _targetWave.WaveSpeed = 0;

                Debug.Log("Failed!");
                return;
            } 
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
        _progressBar.positionCount += 1;

        int lastPosition = _progressBar.positionCount - 1;

        Vector3 position = new Vector3((timer / 15f) * -_progressBarLimits.y, transferSpeed * 5, 0);
        
        _progressBar.SetPosition(lastPosition, position);
    }
}
