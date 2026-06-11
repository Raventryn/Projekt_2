using UnityEngine;
using System.Collections;

public class FadePanelOnStart : MonoBehaviour
{
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
            fullscreenPanelCG.alpha = 1f - Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        fullscreenPanelCG.alpha = 0f;
        fullscreenPanelCG.gameObject.SetActive(false);
    }
}
