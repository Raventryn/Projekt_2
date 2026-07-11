using System.Collections;
using UnityEngine;

public class RoachScanBehaviour : MonoBehaviour
{
    [SerializeField] GameObject _roachObject;
    [SerializeField] Material _rawMaterial;
    [SerializeField] Material _cookedMaterial;
    [SerializeField] ScanObject _scanObject;

    Material _roachMaterial;
    Animator _animator;
    ParticleSystem _particleSystem;
    BoxCollider _collider;

    RoachNPCBehaviour _npcBehaviour;

    float _particlesEmissionAmount = 12f;

    public bool IsScannable;
    public bool IsBeingScanned;
    public float cookedState = 0;

    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onScanObjectOn += StartScanning;
        GameEventsManager.instance.interactionEvents.onScanObjectOff += StopScanning;
        GameEventsManager.instance.questEvents.onAllowRoachScan += AllowRoachScan;
        GameEventsManager.instance.questEvents.onInterpretedObject += UpdateRoachObject;
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onScanObjectOn -= StartScanning;
        GameEventsManager.instance.interactionEvents.onScanObjectOff -= StopScanning;
        GameEventsManager.instance.questEvents.onAllowRoachScan -= AllowRoachScan;
        GameEventsManager.instance.questEvents.onInterpretedObject -= UpdateRoachObject;
    }

    void Start()
    {
        SetReferences(_roachObject);
    }

    void Update()
    {
        if (IsBeingScanned)
        {
            cookedState += 0.5f * Time.deltaTime;
            BlendMaterial(cookedState);

            var emission = _particleSystem.emission;
            emission.rateOverTime = _particlesEmissionAmount * cookedState;

            Debug.Log("Cooking!");

            if(cookedState >= 1f)
            {
                IsBeingScanned = false;
                StopCooking();
            }
        }
    }

    void SetReferences(GameObject gameObject)
    {
        _roachMaterial = gameObject.GetComponent<Renderer>().material;
        _animator = GetComponent<Animator>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _npcBehaviour = GetComponent<RoachNPCBehaviour>();
        _collider = GetComponent<BoxCollider>();
    }

    void StartScanning(GameObject gameObject, ScannerMode mode)
    {
        Debug.Log(gameObject);
        if(gameObject != this.gameObject || mode != ScannerMode.SCAN || !IsScannable) return;

        IsBeingScanned = true;
    }

    void StopScanning(GameObject gameObject, ScannerMode mode )
    {
        if(gameObject != this.gameObject || mode != ScannerMode.SCAN || !IsScannable) return;
        
        IsBeingScanned = false;
    }

    void BlendMaterial(float value)
    {
        _roachMaterial.Lerp(_rawMaterial,_cookedMaterial, value);
    }

    void StopCooking()
    {
        IsScannable = false;
        _animator.SetTrigger("IsCooked");
        _npcBehaviour.StopBehaviour();
        GameEventsManager.instance.questEvents.GrilledRoach();
        ExperienceManager.instance.AddMoney(Random.Range(5, 12));
        //Notify that roach is cooked
    }

    void AllowRoachScan(bool toggle)
    {
        IsScannable = toggle;
        ScanObject scanObject = GetComponentInChildren<ScanObject>();
        if (scanObject._GlitchParticles.isEmitting)
        {
            if (scanObject._GlitchParticles != null)
            {
               scanObject.HideScanGlitch(scanObject.ObjectType);
            scanObject.ChangeMaterial(scanObject.DefaultMaterial); 
            }   
        }
        
        scanObject.ToggleCollider(!toggle);
        scanObject.enabled = !toggle;
        _collider.enabled = toggle;
    }

    void UpdateRoachObject()
    {
        StartCoroutine(DelayGOUpdate());
    }

    IEnumerator DelayGOUpdate()
    {
        yield return null;

        _scanObject = GetComponentInChildren<ScanObject>();
        _roachObject = _scanObject.ScanObjectGO;
        SetReferences(_roachObject);
    }
}
