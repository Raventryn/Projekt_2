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
            if(_glitchController.ScanLineJitter < 0.05 && _IsStrenghtening)
            {
                _glitchController.ScanLineJitter += 0.1f + Time.deltaTime;

                if(_glitchController.ScanLineJitter >= 0.05f)
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
        _IsGlitching = true;
    }
}
