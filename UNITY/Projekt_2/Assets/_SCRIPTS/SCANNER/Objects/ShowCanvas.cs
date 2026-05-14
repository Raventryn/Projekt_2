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

    IEnumerator InfoTextAnimation(bool toggle)
    {
        switch (toggle)
        {
            case true:
                InfoCanvasContainer.SetActive(toggle);

                InfoCanvasAnimator.SetBool("showText", toggle);

                yield return new WaitForSeconds(0.15f);
         
                break;

            case false:
                InfoCanvasAnimator.SetBool("showText", toggle);

                yield return new WaitForSeconds(0.15f);

                InfoCanvasContainer.SetActive(toggle);
                
                break;
        }
        
    }
}
