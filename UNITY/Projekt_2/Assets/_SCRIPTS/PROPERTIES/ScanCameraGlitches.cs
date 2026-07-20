using KinoGlitch;
using UnityEngine;

public class ScanCameraGlitches : MonoBehaviour
{
    [SerializeField] AnalogGlitchController _glitchController;

    bool _IsGlitching;
    bool _IsStrenghtening;

    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onUpdateObjectScannedState += GlitchOnScan;
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onUpdateObjectScannedState -= GlitchOnScan;
    }

    void Update()
    {
        if (_IsGlitching)
        {
            if(_glitchController.ScanLineJitter < 0.1f && _IsStrenghtening)
            {
                _glitchController.ScanLineJitter += 0.1f + Time.deltaTime;

                Debug.Log("Entered");

                if(_glitchController.ScanLineJitter >= 0.1f)
                    _IsStrenghtening = false;
            }
            else
            {
                _glitchController.ScanLineJitter -= 0.1f * Time.deltaTime;

                if(_glitchController.ScanLineJitter <= 0)
                    _IsGlitching = false;
            }
            
        }
    }

    void GlitchOnScan(ScannableObjectType type)
    {
        Debug.Log("Entered");
        _IsGlitching = true;
        _IsStrenghtening = true;

        GameEventsManager.instance.soundEvents.TriggerSound(SoundType.GLITCH, false);
    }
}
