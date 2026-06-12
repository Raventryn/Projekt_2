using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.Video;
using System.Collections.Generic;
using Unity.VisualScripting;

public class OptionsMenuManager : MonoBehaviour
{
    [SerializeField] PlayerSettingsSO _playerSettings;
    [SerializeField] Button _returnButton;
    [SerializeField] GameObject _settingsMenuContainer;
    [SerializeField] GameObject _previousMenuContainer;

    [SerializeField] AudioMixer audioMixer;

    [SerializeField] TMP_Text masterVolumeText;
    [SerializeField] TMP_Text voicesVolumeText;
    [SerializeField] TMP_Text effectsVolumeText;
    [SerializeField] TMP_Text environmentVolumeText;

    [SerializeField] TMP_Dropdown resolutionsDropdown;
    [SerializeField] TMP_Dropdown refreshRatesDropdown;
    [SerializeField] TMP_Dropdown fullscreenModeDropdown;

    List<Resolution> allResolutions = new List<Resolution>();
    List<RefreshRate> refreshRates = new List<RefreshRate>();
    List<Resolution> resolutions = new List<Resolution>();
    List<FullScreenMode> fullScreenModes = new List<FullScreenMode>();

    int screenWidth;
    int screenHeight;
    RefreshRate refreshRate;

    FullScreenMode fullScreenMode;

    float _lookSensitivity;

    void Start()
    {
        _lookSensitivity = _playerSettings.LookSensitivity;

        _returnButton.onClick.AddListener(() => StartCoroutine(DelayButtonAction()));

        SetVolumeTexts();

        _settingsMenuContainer.SetActive(false);

        refreshRate = Screen.currentResolution.refreshRateRatio;

        allResolutions.AddRange(Screen.resolutions);

        UpdateAvailableRefreshRates();

        UpdateAvailableResolutions();

        UpdateFullScreenModes();
    }

    void ReturnToMainMenu()
    {
        _settingsMenuContainer.SetActive(false);
        _previousMenuContainer.SetActive(true);
    }

    void UpdateScreenResolution()
    {
        Screen.SetResolution(screenWidth, screenHeight, fullScreenMode, refreshRate);

        _playerSettings.ScreenResolution = Screen.currentResolution;
        _playerSettings.fullscreenMode = Screen.fullScreenMode;
    }

    void UpdateAvailableResolutions()
    {
        foreach(Resolution res in allResolutions)
        {
            if(res.refreshRateRatio.value == refreshRate.value)
                resolutions.Add(res);
        }

        resolutionsDropdown.ClearOptions();

        for(int i = 0; i < resolutions.Count; i++)
        {
            resolutionsDropdown.options.Add(new TMP_Dropdown.OptionData( $"{resolutions[i].width}" + " x " + $"{resolutions[i].height}"));
        }

        resolutionsDropdown.value = resolutions.IndexOf(Screen.currentResolution);

        resolutionsDropdown.RefreshShownValue();
    }

    void UpdateAvailableRefreshRates()
    {
        refreshRatesDropdown.ClearOptions();

        foreach(Resolution res in allResolutions)
        {
            if (!refreshRates.Contains(res.refreshRateRatio))
            {
                refreshRates.Add(res.refreshRateRatio);
                refreshRatesDropdown.options.Add(new TMP_Dropdown.OptionData($"{res.refreshRateRatio.value}"));
            }
        }

        refreshRatesDropdown.value = refreshRates.IndexOf(Screen.currentResolution.refreshRateRatio);

        refreshRatesDropdown.RefreshShownValue();
    }

    void UpdateFullScreenModes()
    {
        fullscreenModeDropdown.ClearOptions();

        fullScreenModes.Add(FullScreenMode.Windowed);
        fullscreenModeDropdown.options.Add(new TMP_Dropdown.OptionData("Windowed"));
        fullScreenModes.Add(FullScreenMode.FullScreenWindow);
        fullscreenModeDropdown.options.Add(new TMP_Dropdown.OptionData("Fullscreen Window"));
        fullScreenModes.Add(FullScreenMode.ExclusiveFullScreen);
        fullscreenModeDropdown.options.Add(new TMP_Dropdown.OptionData("Fullscreen"));

        fullscreenModeDropdown.value = fullScreenModes.IndexOf(Screen.fullScreenMode);

        fullscreenModeDropdown.RefreshShownValue();
    }

    public void SetResolution(int index)
    {
        screenWidth = resolutions[index].width;
        screenHeight = resolutions[index].height;
        UpdateScreenResolution();
    }

    public void SetFullscreenMode(int index)
    {
        fullScreenMode = fullScreenModes[index];

        UpdateScreenResolution();
    }

    public void SetRefreshRate(int index)
    {
        refreshRate = refreshRates[index];
        UpdateAvailableResolutions();
        UpdateScreenResolution();
    }

    public void ChangeLookSensitivity(float value)
    {
        _lookSensitivity = Mathf.Clamp(2f + (value * 13f), 2, 15);
        _playerSettings.LookSensitivity = _lookSensitivity;
    }

    public void ChangeMasterVolume(float value)
    {
        float correctedValue = -80 + value * 100;

        audioMixer.SetFloat("volumeMaster", correctedValue);

        masterVolumeText.text = "Master Volume: " + (value * 100).ToString("F0");

        _playerSettings.MasterVolume = correctedValue;
    }

    public void ChangeVoicesVolume(float value)
    {
        float correctedValue = -80 + value * 80;

        audioMixer.SetFloat("volumeVoices", correctedValue);

        voicesVolumeText.text = "Voices Volume: " + (value * 100).ToString("F0");

        _playerSettings.VoicesVolume = correctedValue;
    }

    public void ChangeEffectsVolume(float value)
    {
        float correctedValue = -80 + value * 80;

        audioMixer.SetFloat("volumeEffects", correctedValue);

        effectsVolumeText.text = "Effects Volume: " + (value * 100).ToString("F0");

        _playerSettings.EffectsVolume = correctedValue;
    }

    public void ChangeEnvironmentVolume(float value)
    {
        float correctedValue = -80 + value * 80;

        audioMixer.SetFloat("volumeEnvironment", correctedValue);

        environmentVolumeText.text = "Environment Volume: " + (value * 100).ToString("F0");

        _playerSettings.EnvironmentVolume = correctedValue;
    }

    void SetVolumeTexts()
    {
        masterVolumeText.text = "Master Volume: 50";
        voicesVolumeText.text = "Voices Volume: 100";
        effectsVolumeText.text = "Effects Volume: 100";
        environmentVolumeText.text = "Environment Volume: 100";
    }

    IEnumerator DelayButtonAction()
    {
        yield return new WaitForSeconds(0.25f);

        ReturnToMainMenu();
    }
}
