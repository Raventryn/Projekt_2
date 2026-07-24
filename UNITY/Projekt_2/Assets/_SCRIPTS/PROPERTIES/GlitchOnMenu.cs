using KinoGlitch;
using UnityEngine;

public class GlitchOnMenu : MonoBehaviour
{
    DigitalGlitchController digitalGlitchController;

    bool isGlitching;
    bool isIncreasing;

    private void OnEnable()
    {
        GameEventsManager.instance.uiEvents.onGlitchOnMenu += Glitch;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.uiEvents.onGlitchOnMenu -= Glitch;
    }

    private void Start()
    {
        digitalGlitchController = GetComponent<DigitalGlitchController>();
    }

    private void Update()
    {
        if (isGlitching)
        {
            /*if (isIncreasing)
            {
                digitalGlitchController.Intensity += 2 * Time.deltaTime;

                if(digitalGlitchController.Intensity >= 1)
                {
                    isIncreasing = false;
                }
            }
            else
            {*/
                digitalGlitchController.Intensity -= 2 * Time.deltaTime;

                if( digitalGlitchController.Intensity <= 0)
                {
                    isGlitching = false;
                }
            //}
        }
    }

    void Glitch()
    {
        isGlitching = true;
        digitalGlitchController.Intensity = 1;

        GameEventsManager.instance.soundEvents.TriggerSound(SoundType.GLITCH, false);
        //isIncreasing = true;
    }
}
