using UnityEngine;
using System.Collections;
using TMPro;
using Febucci.TextAnimatorForUnity;

public class ShowCanvas : MonoBehaviour
{
    public TMP_Text TmpText;
    public TypewriterComponent Typewriter;
    public GameObject InfoCanvasContainer;
    public Animator InfoCanvasAnimator;
    public string DescriptionText;

    public Vector3 canvasDefaultPosition;

    void Start()
    {
        canvasDefaultPosition = InfoCanvasContainer.transform.position;
    }

    public void ShowInformationCanvas(bool firstTimeShowing)
    {
        StopAllCoroutines();
        StartCoroutine(InfoTextAnimation(true));

        if (firstTimeShowing)
        {
            Typewriter.ShowText(DescriptionText);

            Typewriter.StartShowingText();
        }    
    }

    public void HideInformationCanvas()
    {
        StopAllCoroutines();
        StartCoroutine(InfoTextAnimation(false));

        if(TmpText.text != DescriptionText)
        {
            TmpText.text = "";
        }
    }

    public Vector3 SetCanvasWorldPosition()
    {
        Vector2 viewportPoint = Camera.main.ViewportToScreenPoint(new Vector2(0.25f, 0.5f));

        Vector3 newPoint = Camera.main.ScreenToWorldPoint(new Vector3(viewportPoint.x, viewportPoint.y, 1f));

        return newPoint;
    }

    IEnumerator InfoTextAnimation(bool toggle)
    {
        switch (toggle)
        {
            case true:
                InfoCanvasContainer.transform.position = SetCanvasWorldPosition();

                InfoCanvasContainer.SetActive(toggle);

                InfoCanvasAnimator.SetBool("showText", toggle);

                yield return new WaitForSeconds(0.15f);
         
                break;

            case false:
                InfoCanvasAnimator.SetBool("showText", toggle);

                yield return new WaitForSeconds(0.15f);

                InfoCanvasContainer.transform.position = canvasDefaultPosition;

                InfoCanvasContainer.SetActive(toggle);
                
                break;
        }
        
    }
}
