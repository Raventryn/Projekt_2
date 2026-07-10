using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreditsMenuManager : MonoBehaviour
{
    [SerializeField] Button returnButton;
    [SerializeField] GameObject mainMenuContainer;
    [SerializeField] GameObject creditsMenuContainer;
    void Awake()
    {
        returnButton.onClick.AddListener(() => StartCoroutine(DelayButtonAction()));
        creditsMenuContainer.SetActive(false);
    }

    void ReturnToMainMenu()
    {
        creditsMenuContainer.SetActive(false);
        mainMenuContainer.SetActive(true);
    }

    IEnumerator DelayButtonAction()
    {
        yield return new WaitForSeconds(0.25f);

        ReturnToMainMenu();
    }
}
