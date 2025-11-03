using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light targetLight;
    public Renderer bulbRenderer;
    public float flickerSpeed = 5f;
    public float maxIntensity = 3f;
    public float minIntensity = 0.2f;

    private Material bulbMaterial;

    void Start()
    {
        if (bulbRenderer != null)
            bulbMaterial = bulbRenderer.material;
    }

    void Update()
    {
        float flicker = Mathf.Abs(Mathf.Sin(Time.time * flickerSpeed));
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, flicker);

        if (targetLight != null)
            targetLight.intensity = intensity;

        if (bulbMaterial != null)
            bulbMaterial.SetFloat("_EmissionIntensity", intensity * 3f);
    }
}
