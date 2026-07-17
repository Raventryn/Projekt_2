using UnityEngine;

public class Rotate : MonoBehaviour
{

    [SerializeField] private float speed;

    void Update()
    {
        gameObject.transform.Rotate(0,speed*Time.deltaTime,0);
    }
}
