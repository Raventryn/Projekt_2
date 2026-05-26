using UnityEngine;

public class RoachScanBehaviour : MonoBehaviour
{
    [SerializeField] Material _rawMaterial;
    [SerializeField] Material _cookedMaterial;

    Material _roachMaterial;
    Animator _animator;
    ParticleSystem _particleSystem;

    RoachNPCBehaviour _npcBehaviour;

    float _particlesEmissionAmount = 12f;

    public bool IsBeingScanned;
    public float cookedState = 0;

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
        _npcBehaviour = GetComponent<RoachNPCBehaviour>();
    }

    void Update()
    {
        if (IsBeingScanned)
        {
            cookedState += 0.5f * Time.deltaTime;
            BlendMaterial(cookedState);

            var emission = _particleSystem.emission;
            emission.rateOverTime = _particlesEmissionAmount * cookedState;

            if(cookedState >= 1f)
            {
                IsBeingScanned = false;
                StopCooking();
            }
        }
    }

    void StartScanning(GameObject gameObject, ScannerMode mode)
    {
        if(gameObject != this.gameObject || mode != ScannerMode.SCAN) return;

        IsBeingScanned = true;
    }

    void StopScanning(GameObject gameObject, ScannerMode mode)
    {
        if(gameObject != this.gameObject || mode != ScannerMode.SCAN) return;
        
        IsBeingScanned = false;
    }

    void BlendMaterial(float value)
    {
        _roachMaterial.Lerp(_rawMaterial,_cookedMaterial, value);
    }

    void StopCooking()
    {
        _animator.SetTrigger("IsCooked");
        _npcBehaviour.StopBehaviour();
        //Notify that roach is cooked
    }
}
