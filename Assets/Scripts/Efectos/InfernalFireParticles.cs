using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class InfernalFireParticles : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("El objeto del diablo desde donde salen las partículas")]
    public Transform demonObject;

    [Tooltip("Cuántas partículas por segundo")]
    [Range(10, 500)]
    public int emissionRate = 150;

    [Header("Particle Appearance")]
    [Tooltip("Tamaño de las brasas/llamas")]
    [Range(0.05f, 1f)]
    public float particleSize = 0.15f;

    [Tooltip("Color rojo infernal base")]
    public Color infernalRed = new Color(1f, 0.1f, 0f, 1f);

    [Tooltip("Color naranja de fuego")]
    public Color fireOrange = new Color(1f, 0.4f, 0f, 1f);

    [Tooltip("Color amarillo brillante")]
    public Color brightYellow = new Color(1f, 0.9f, 0.2f, 1f);

    [Header("Movement")]
    [Tooltip("Velocidad de subida de las partículas")]
    [Range(0.5f, 5f)]
    public float upwardSpeed = 2f;

    [Tooltip("Movimiento aleatorio lateral")]
    [Range(0f, 3f)]
    public float randomMovement = 0.8f;

    [Header("Behavior")]
    [Tooltip("Vida de cada partícula")]
    [Range(0.5f, 5f)]
    public float particleLifetime = 2f;

    [Tooltip("Distancia desde la superficie del diablo")]
    [Range(0f, 2f)]
    public float spawnDistance = 0.2f;

    private ParticleSystem ps;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.ShapeModule shapeModule;
    private ParticleSystem.VelocityOverLifetimeModule velocityModule;
    private ParticleSystem.ColorOverLifetimeModule colorModule;
    private ParticleSystem.SizeOverLifetimeModule sizeModule;
    private ParticleSystem.LightsModule lightsModule;

    void Start()
    {
        SetupParticleSystem();
    }

    void SetupParticleSystem()
    {
        ps = GetComponent<ParticleSystem>();

        // Main Module - Configuración base
        mainModule = ps.main;
        mainModule.startLifetime = particleLifetime;
        mainModule.startSpeed = new ParticleSystem.MinMaxCurve(upwardSpeed * 0.5f, upwardSpeed * 1.5f);
        mainModule.startSize = new ParticleSystem.MinMaxCurve(particleSize * 0.5f, particleSize * 2f);
        mainModule.startColor = infernalRed;
        mainModule.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
        mainModule.maxParticles = 1000;
        mainModule.gravityModifier = -0.2f; // Flotación ligera

        // Emission Module - Cuántas partículas
        emissionModule = ps.emission;
        emissionModule.rateOverTime = emissionRate;

        // Shape Module - Desde dónde salen
        shapeModule = ps.shape;
        shapeModule.enabled = true;

        if (demonObject != null)
        {
            // Si hay mesh, emitir desde la superficie
            MeshRenderer meshRenderer = demonObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                shapeModule.shapeType = ParticleSystemShapeType.MeshRenderer;
                shapeModule.meshRenderer = meshRenderer;
                shapeModule.meshShapeType = ParticleSystemMeshShapeType.Triangle;
            }
            else
            {
                // Si no hay mesh, emitir desde esfera alrededor del objeto
                shapeModule.shapeType = ParticleSystemShapeType.Sphere;
                shapeModule.radius = 1f;
            }
        }
        else
        {
            shapeModule.shapeType = ParticleSystemShapeType.Sphere;
            shapeModule.radius = 1f;
        }

        shapeModule.radiusThickness = 1f; // Emitir desde superficie

        // Velocity Over Lifetime - Movimiento
        velocityModule = ps.velocityOverLifetime;
        velocityModule.enabled = true;
        velocityModule.space = ParticleSystemSimulationSpace.World;

        // Movimiento hacia arriba con ondulación
        AnimationCurve upwardCurve = new AnimationCurve();
        upwardCurve.AddKey(0f, upwardSpeed);
        upwardCurve.AddKey(1f, upwardSpeed * 1.5f);
        velocityModule.y = new ParticleSystem.MinMaxCurve(1f, upwardCurve);

        // Movimiento lateral aleatorio
        velocityModule.x = new ParticleSystem.MinMaxCurve(-randomMovement, randomMovement);
        velocityModule.z = new ParticleSystem.MinMaxCurve(-randomMovement, randomMovement);

        // Color Over Lifetime - Rojo → Naranja → Amarillo → Transparente
        colorModule = ps.colorOverLifetime;
        colorModule.enabled = true;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(infernalRed, 0.0f),
                new GradientColorKey(fireOrange, 0.3f),
                new GradientColorKey(brightYellow, 0.6f),
                new GradientColorKey(brightYellow, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(1.0f, 0.5f),
                new GradientAlphaKey(0.5f, 0.8f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );
        colorModule.color = gradient;

        // Size Over Lifetime - Las partículas se encogen al morir
        sizeModule = ps.sizeOverLifetime;
        sizeModule.enabled = true;

        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 1f);
        sizeCurve.AddKey(0.5f, 1.2f); // Crecen un poco
        sizeCurve.AddKey(1f, 0f); // Desaparecen
        sizeModule.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // Renderer
        ParticleSystemRenderer renderer = GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = CreateParticleMaterial();
        renderer.sortingOrder = 10;

        Debug.Log("🔥 Infernal Fire Particles configurado! Asegúrate de asignar el 'demonObject' en el Inspector.");
    }

    Material CreateParticleMaterial()
    {
        // Crear material aditivo para partículas brillantes
        Material mat = new Material(Shader.Find("Particles/Standard Unlit"));
        mat.SetFloat("_Mode", 3); // Transparent mode
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One); // Additive
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;

        // Color emisivo
        mat.SetColor("_EmissionColor", new Color(2f, 0.5f, 0f, 1f));
        mat.EnableKeyword("_EMISSION");

        return mat;
    }

    void Update()
    {
        // Actualizar posición si el demonio se mueve
        if (demonObject != null)
        {
            transform.position = demonObject.position;
        }
    }

    // Llamar esto para crear una explosión de fuego
    public void FireBurst(int particleCount = 50)
    {
        ps.Emit(particleCount);
    }
}