using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button quitButton;

    [SerializeField] CanvasGroup fullscreenPanelCG; 
    [SerializeField] float fadeDuration;

    [SerializeField] GameObject pauseMenuContainer;
    [SerializeField] GameObject settingsMenuContainer;

    [SerializeField] Scene gameScene;

    InputEventContext _previousContext;

    bool _isGamePaused;

    event Action onFinishedFade;

    void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onPressedEscape += PauseOrContinueGame;
    }

    void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onPressedEscape -= PauseOrContinueGame;
    }

    void Start()
    {
        AddOnClickEvents();
        StartCoroutine(FadeIn());
        pauseMenuContainer.SetActive(false);
    }

    void AddOnClickEvents()
    {
        continueButton.onClick.AddListener(() => StartButtonAction(ButtonType.START));
        settingsButton.onClick.AddListener(() => StartButtonAction(ButtonType.OPTIONS));
        quitButton.onClick.AddListener(() => StartButtonAction(ButtonType.QUIT));
    }

    void PauseOrContinueGame(InputEventContext context)
    {
        if(context != InputEventContext.DEFAULT && context != InputEventContext.MENU) return;

        if (!_isGamePaused)
        {
            GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.MENU);
            GameEventsManager.instance.playerEvents.TogglePlayerCamera(false);
            GameEventsManager.instance.playerEvents.TogglePlayerMovement(false);

            GameEventsManager.instance.inputEvents.ShowCursor(true);

            _isGamePaused = true;
            pauseMenuContainer.SetActive(true);
        }
        else
        {
            _isGamePaused = false;
            pauseMenuContainer.SetActive(false);
            GameEventsManager.instance.inputEvents.ChangeInputContext(_previousContext);

            GameEventsManager.instance.playerEvents.TogglePlayerCamera(true);
            GameEventsManager.instance.playerEvents.TogglePlayerMovement(true);

            GameEventsManager.instance.inputEvents.ShowCursor(false);
        }
        
    }

    void ShowOptions()
    {
        pauseMenuContainer.SetActive(false);
        settingsMenuContainer.SetActive(true);
    }

    void QuitGame()
    {
        onFinishedFade -= QuitGame;
        SceneManager.LoadScene("MainMenu_Test");
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
                PauseOrContinueGame(InputEventContext.MENU);
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
