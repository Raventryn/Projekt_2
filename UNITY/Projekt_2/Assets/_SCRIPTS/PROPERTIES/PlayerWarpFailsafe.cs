using UnityEngine;

public class PlayerWarpFailsafe : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController charController = other.gameObject.GetComponent<CharacterController>();
            charController.enabled = false;
            other.gameObject.transform.position = new Vector3(-7, 0, 0.3f);
            charController.enabled = true;
        }
    }
}
