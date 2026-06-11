using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

enum ButtonType
{
    START,
    OPTIONS,
    QUIT
}
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button quitButton;
    [SerializeField] CanvasGroup fullscreenPanelCG; 
    [SerializeField] float fadeDuration;
    [SerializeField] GameObject mainMenuContainer;
    [SerializeField] GameObject settingsMenuContainer;
    [SerializeField] Scene gameScene;

    event Action onFinishedFade;

    void Start()
    {
        GameEventsManager.instance.inputEvents.ShowCursor(true);
        AddOnClickEvents();
        StartCoroutine(FadeIn());
    }

    void AddOnClickEvents()
    {
        startButton.onClick.AddListener(() => StartButtonAction(ButtonType.START));
        optionsButton.onClick.AddListener(() => StartButtonAction(ButtonType.OPTIONS));
        quitButton.onClick.AddListener(() => StartButtonAction(ButtonType.QUIT));
    }

    void StartGame()
    {
        onFinishedFade -= StartGame;
        SceneManager.LoadScene("JasminTestlevel");
    }

    void ShowOptions()
    {
        mainMenuContainer.SetActive(false);
        settingsMenuContainer.SetActive(true);
    }

    void QuitGame()
    {
        onFinishedFade -= QuitGame;
        Application.Quit();
    }

    void StartButtonAction(ButtonType type)
    {
        StartCoroutine(DelayButtonAction(type));
    }

    void ExecuteButtonAction(ButtonType type)
    {
        switch (type)
        {
            case ButtonType.START:
                onFinishedFade += StartGame;
                StartCoroutine(FadeOut());
                break;
            case ButtonType.OPTIONS:
                ShowOptions();
                break;
            case ButtonType.QUIT:
                onFinishedFade += QuitGame;
                StartCoroutine(FadeOut());
                break;
        }
    }

    IEnumerator DelayButtonAction(ButtonType type)
    {
        yield return new WaitForSeconds(0.25f);

        ExecuteButtonAction(type);
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

        onFinishedFade?.Invoke();
    }

    IEnumerator FadeOut()
    {
        float t = 0;

        fullscreenPanelCG.gameObject.SetActive(true);

        while(t < fadeDuration)
        {
            t += Time.deltaTime;
            fullscreenPanelCG.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        fullscreenPanelCG.alpha = 1;

        onFinishedFade?.Invoke();
    }
}
