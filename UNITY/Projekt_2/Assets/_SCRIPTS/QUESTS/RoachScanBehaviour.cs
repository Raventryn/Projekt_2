using UnityEngine;

public class RoachScanBehaviour : MonoBehaviour
{
    [SerializeField] Material _rawMaterial;
    [SerializeField] Material _cookedMaterial;

    Material _roachMaterial;
    Animator _animator;
    ParticleSystem _particleSystem;

    bool _IsBeingScanned;
    float _cookedState = 0;

    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onScanObjectOn += StartScanning;
        GameEventsManager.instance.interactionEvents.onScanObjectOff += StopScanning;
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onScanObjectOn -= StartScanning;
        GameEventsManager.instance.interactionEvents.onScanObjectOff += StopScanning;
    }

    void Start()
    {
        _roachMaterial = GetComponent<Renderer>().material;
        _animator = GetComponent<Animator>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (_IsBeingScanned)
        {
            _cookedState += 0.3f * Time.deltaTime;
            BlendMaterial(_cookedState);

            if(_cookedState >= 1f)
            {
                _IsBeingScanned = false;
                StopCooking();
            }
        }
    }

    void StartScanning(GameObject gameObject, ScannerMode mode)
    {
        if(gameObject != this.gameObject || mode != ScannerMode.SCAN) return;

        _IsBeingScanned = true;
    }

    void StopScanning(GameObject gameObject, ScannerMode mode)
    {
        if(gameObject != this.gameObject || mode != ScannerMode.SCAN) return;
        
        _IsBeingScanned = false;
    }

    void BlendMaterial(float value)
    {
        _roachMaterial.Lerp(_rawMaterial,_cookedMaterial, value);
    }

    void StopCooking()
    {
        _animator.SetTrigger("IsCooked");
        //Stop Roach Movement
    }
}
