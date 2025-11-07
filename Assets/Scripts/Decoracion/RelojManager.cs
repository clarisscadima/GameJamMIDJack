using System.Collections;
using UnityEngine;

public class RelojManager : MonoBehaviour
{
    [Header("REFERENCIAS - Arrastra aquí")]
    public Transform circuloBase;
    public Transform cuboHora;
    public Transform cuboMinuto;
    public AudioClip sonidoAlarma;

    [Header("CONFIGURACIÓN")]
    public float radio = 3f;
    public float intervaloAlarma = 10f;
    public bool usarTiempoReal = true;

    private AudioSource audioSource;
    private float tiempoTranscurrido = 0f;
    private float tiempoTotal = 0f;

    void Start()
    {
        // Crear componente de audio
        audioSource = gameObject.AddComponent<AudioSource>();
        if (sonidoAlarma != null)
            audioSource.clip = sonidoAlarma;

        // Posicionar cubos
        ColocarCubosEnPosicion();

        Debug.Log("Reloj iniciado correctamente");
    }

    void Update()
    {
        // Actualizar tiempo
        float deltaTiempo = usarTiempoReal ? Time.deltaTime : Time.unscaledDeltaTime;
        tiempoTranscurrido += deltaTiempo;
        tiempoTotal += deltaTiempo;

        // Mover manecillas
        MoverManecillas();

        // Verificar alarma
        if (tiempoTranscurrido >= intervaloAlarma)
        {
            tiempoTranscurrido = 0f;
            SonarAlarma();
        }
    }

    void ColocarCubosEnPosicion()
    {
        if (circuloBase == null) return;

        Vector3 centro = circuloBase.position;

        // Cubo hora (manecilla corta)
        if (cuboHora != null)
        {
            cuboHora.position = centro + Vector3.forward * radio * 0.6f;
            cuboHora.localScale = new Vector3(0.15f, 0.15f, 1.5f);
        }

        // Cubo minuto (manecilla larga)
        if (cuboMinuto != null)
        {
            cuboMinuto.position = centro + Vector3.forward * radio * 0.9f;
            cuboMinuto.localScale = new Vector3(0.1f, 0.1f, 2.0f);
        }
    }

    void MoverManecillas()
    {
        if (circuloBase == null || cuboHora == null || cuboMinuto == null) return;

        Vector3 centro = circuloBase.position;

        // Calcular rotaciones
        float minutosTotales = tiempoTotal / 60f;
        float horasTotales = minutosTotales / 12f;

        // Minutos (360° cada 60 minutos)
        float anguloMinutos = (minutosTotales % 60f) * 6f;
        cuboMinuto.position = centro;
        cuboMinuto.rotation = Quaternion.Euler(0f, -anguloMinutos, 0f);
        cuboMinuto.Translate(0f, 0f, radio * 0.9f);

        // Horas (360° cada 12 horas)
        float anguloHoras = (horasTotales % 12f) * 30f;
        cuboHora.position = centro;
        cuboHora.rotation = Quaternion.Euler(0f, -anguloHoras, 0f);
        cuboHora.Translate(0f, 0f, radio * 0.6f);
    }

    void SonarAlarma()
    {
        Debug.Log("🔔 ALARMA! Tiempo: " + FormatearTiempo());

        if (audioSource != null && sonidoAlarma != null)
            audioSource.Play();

        StartCoroutine(EfectoAlarma());
    }

    System.Collections.IEnumerator EfectoAlarma()
    {
        Renderer renderer = circuloBase.GetComponent<Renderer>();
        if (renderer != null)
        {
            Color original = renderer.material.color;
            renderer.material.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            renderer.material.color = original;
        }
    }

    string FormatearTiempo()
    {
        int horas = (int)(tiempoTotal / 3600f);
        int minutos = (int)((tiempoTotal % 3600f) / 60f);
        int segundos = (int)(tiempoTotal % 60f);
        return $"{horas:00}:{minutos:00}:{segundos:00}";
    }
}