using UnityEngine;
using System.Collections;
using KinoGlitch;

public class FadePanelOnStart : MonoBehaviour
{
    [SerializeField] DigitalGlitchController glitchController;
    [SerializeField] CanvasGroup fullscreenPanelCG;
    [SerializeField] float fadeDuration;
    GameObject panelGO;

    void Start()
    {
        panelGO = fullscreenPanelCG.gameObject;
        panelGO.SetActive(true);
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0;

        while(t < fadeDuration)
        {
            t+= Time.deltaTime;
            //fullscreenPanelCG.alpha = 1f - Mathf.Clamp01(t / fadeDuration);
            glitchController.Intensity = 1 - Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        glitchController.Intensity = 0;
        fullscreenPanelCG.alpha = 0f;
        fullscreenPanelCG.gameObject.SetActive(false);
    }
}
