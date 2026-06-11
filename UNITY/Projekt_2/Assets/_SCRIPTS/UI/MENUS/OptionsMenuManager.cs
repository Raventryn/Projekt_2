using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionsMenuManager : MonoBehaviour
{
    [SerializeField] PlayerSettingsSO _playerSettings;
    [SerializeField] Button _returnButton;
    [SerializeField] GameObject _settingsMenuContainer;
    [SerializeField] GameObject _previousMenuContainer;

    float _lookSensitivity;

    void Start()
    {
        _lookSensitivity = _playerSettings.LookSensitivity;

        _returnButton.onClick.AddListener(() => StartCoroutine(DelayButtonAction()));
    }

    void ReturnToMainMenu()
    {
        _settingsMenuContainer.SetActive(false);
        _previousMenuContainer.SetActive(true);
    }

    public void ChangeLookSensitivity(float value)
    {
        _lookSensitivity = Mathf.Clamp(2f + (value * 13f), 2, 15);
        _playerSettings.LookSensitivity = _lookSensitivity;
    }

    IEnumerator DelayButtonAction()
    {
        yield return new WaitForSeconds(0.25f);

        ReturnToMainMenu();
    }
}
