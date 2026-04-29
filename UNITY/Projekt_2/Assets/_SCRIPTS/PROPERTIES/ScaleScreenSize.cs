using UnityEngine;

public class ScaleScreenSize : MonoBehaviour
{
    [Range (1f, 10f)]
    [SerializeField] float _scaleFactor;
    Camera _playerCamera;

    void Start()
    {
        _playerCamera = Camera.main;
    }

    void Update()
    {
        ScaleToDistance();
    }

    void ScaleToDistance()
    {
        Vector3 direction = Camera.main.transform.position - transform.position;

        float distance = direction.magnitude;

        transform.localScale = new Vector3(distance, distance, distance) * 0.001f * _scaleFactor;
    }
}
