using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StartGlitchesBehaviour : MonoBehaviour
{
    [SerializeField] List<GameObject> glitchObjects = new List<GameObject>();
    [SerializeField] Material glitchMaterial;
    List<ParticleSystem> particlesSystems = new List<ParticleSystem>();
    List<Renderer> objectRenderers = new List<Renderer>();
    List<Material> objectMaterials = new List<Material>();
    
    void OnEnable()
    {
        GameEventsManager.instance.questEvents.onDisableGlitches += DisableGlitch;
    }

    void OnDisable()
    {
        GameEventsManager.instance.questEvents.onDisableGlitches -= DisableGlitch;
    }


    void Start()
    {
        foreach(GameObject gameObject in glitchObjects)
        {
            particlesSystems.Add(gameObject.GetComponentInChildren<ParticleSystem>());
            Renderer renderer = gameObject.GetComponent<Renderer>();
            objectRenderers.Add(renderer);
            objectMaterials.Add(renderer.material);
            renderer.material = glitchMaterial;
        }
    }

    void DisableGlitch()
    {
        foreach(ParticleSystem particles in particlesSystems)
        {
            particles.gameObject.SetActive(false);
        }

        for(int i = 0; i < objectRenderers.Count; i++)
        {
            objectRenderers[i].material = objectMaterials[i];
        }
    }
}
