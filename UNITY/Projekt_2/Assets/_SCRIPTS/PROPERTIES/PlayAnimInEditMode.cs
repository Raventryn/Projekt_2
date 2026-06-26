using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class PlayAnimInEditMode : MonoBehaviour
{
    [SerializeField] Animator animator;

    [SerializeField] bool animatorPlaying = false;
    void Update()
    {
        if (!animatorPlaying)
        {
            Debug.Log("Playing");
            //animator.AnimationMode
            animatorPlaying = true;
        }
    }
}
