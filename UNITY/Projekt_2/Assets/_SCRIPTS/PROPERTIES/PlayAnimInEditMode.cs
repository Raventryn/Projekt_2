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
            animator.Play("Old Man Idle", 0);
            animatorPlaying = true;
        }
    }
}
