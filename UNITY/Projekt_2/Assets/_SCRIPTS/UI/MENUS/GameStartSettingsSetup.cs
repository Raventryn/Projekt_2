using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameStartSettingsSetup : MonoBehaviour
{
    public static GameStartSettingsSetup instance {get; private set;}

    [SerializeField] PlayerSettingsSO _playerSettings;
    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        } 

        instance = this;

        DontDestroyOnLoad(gameObject);

        RefreshRate refreshRate = Screen.currentResolution.refreshRateRatio;
        FullScreenMode fullScreenMode = FullScreenMode.FullScreenWindow;
        List<Resolution> allResolutions = new List<Resolution>();
        List<Resolution> resolutions = new List<Resolution>();
        allResolutions.AddRange(Screen.resolutions);

        foreach(Resolution res in allResolutions)
        {
            if(res.refreshRateRatio.value == refreshRate.value && !resolutions.Contains(res))
                resolutions.Add(res);
        }

        Resolution resolution = resolutions[resolutions.Count -1];

        //Resolution resolution = new Resolution();

        //resolution.width = Display.main.systemWidth;
        //resolution.height = Display.main.systemHeight;
        //resolution.refreshRateRatio = Screen.currentResolution.refreshRateRatio;

        Debug.LogWarning(resolution);

        Screen.SetResolution(resolution.width, resolution.height, fullScreenMode, resolution.refreshRateRatio);

        PlayerPrefs.SetFloat("ScreenWidth", resolution.width);
        PlayerPrefs.SetFloat("ScreenHeight", resolution.height);
        PlayerPrefs.SetFloat("RefreshRate", (float)resolution.refreshRateRatio.value);

        PlayerPrefs.Save();

        //_playerSettings.ScreenResolution = resolution;
        //_playerSettings.fullScreenMode = fullScreenMode;

        Debug.Log(Screen.currentResolution);
    
        QualitySettings.vSyncCount = 1;
    }
}
