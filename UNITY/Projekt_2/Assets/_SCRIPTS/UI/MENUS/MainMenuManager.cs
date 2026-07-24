using System;
using System.Collections;
using KinoGlitch;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

enum ButtonType
{
    START,
    OPTIONS,
    QUIT,
    CREDITS
}
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button creditsButton;
    [SerializeField] DigitalGlitchController glitchController;
    [SerializeField] CanvasGroup fullscreenPanelCG; 
    [SerializeField] CanvasGroup codeBlockerPanel;
    [SerializeField] float fadeDuration;
    [SerializeField] GameObject mainMenuContainer;
    [SerializeField] GameObject settingsMenuContainer;
    [SerializeField] GameObject creditsMenuContainer;
    [SerializeField] Scene gameScene;

    bool isGameStarted = false;

    event Action onFinishedFade;

    void Start()
    {
        GameEventsManager.instance.inputEvents.ShowCursor(true);
        AddOnClickEvents();
        //StartCoroutine(FadeIn(fullscreenPanelCG));
    }

    void AddOnClickEvents()
    {
        startButton.onClick.AddListener(() => StartButtonAction(ButtonType.START));
        optionsButton.onClick.AddListener(() => StartButtonAction(ButtonType.OPTIONS));
        quitButton.onClick.AddListener(() => StartButtonAction(ButtonType.QUIT));
        creditsButton.onClick.AddListener(() => StartButtonAction(ButtonType.CREDITS));
    }

    public void FadeOutBlockerPanel()
    {
        if (isGameStarted) return;

        isGameStarted = true;
        StartCoroutine(FadeIn(codeBlockerPanel));
        GameEventsManager.instance.soundEvents.TriggerSound(SoundType.MAIN_MENU_MUSIC, true);
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

    void ShowCredits()
    {
        mainMenuContainer.SetActive(false);
        creditsMenuContainer.SetActive(true);
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
            case ButtonType.CREDITS:
                ShowCredits();
                break;
        }
    }

    IEnumerator DelayButtonAction(ButtonType type)
    {
        yield return new WaitForSeconds(0.25f);

        ExecuteButtonAction(type);
    }

    IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float t = 0;

        canvasGroup.alpha = 1f;
        canvasGroup.gameObject.SetActive(true);

        while(t < fadeDuration)
        {
            t+= Time.deltaTime;
            //glitchController.Intensity = 1f - Mathf.Clamp01(t / fadeDuration);
            canvasGroup.alpha = 1f - Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        //glitchController.Intensity = 0f;
        canvasGroup.alpha = 0f;
        canvasGroup.gameObject.SetActive(false);

        onFinishedFade?.Invoke();
    }

    IEnumerator FadeOut()
    {
        float t = 0;

        //fullscreenPanelCG.gameObject.SetActive(true);

        while(t < fadeDuration)
        {
            t += Time.deltaTime;
            //fullscreenPanelCG.alpha = Mathf.Clamp01(t / fadeDuration);
            glitchController.Intensity = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        glitchController.Intensity = 1;
        //fullscreenPanelCG.alpha = 1;

        onFinishedFade?.Invoke();
    }
}
