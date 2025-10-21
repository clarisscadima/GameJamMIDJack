using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("Textos UI - TextMesh Pro")]
    public TMP_Text vidaText;
    public TMP_Text corduraText;
    public TMP_Text fichasText;

    [Header("Estadísticas Player")]
    public int vida = 100;
    public int cordura = 100;
    public int fichas = 0;

    [Header("Límites")]
    public int maxVida = 100;
    public int maxCordura = 100;

    [Header("Mensajes Temporales")]
    public TextMeshProUGUI mensajeTexto;
    public float tiempoMensaje = 3f;

    void Start()
    {
        // Inicializar con valores por defecto
        vida = maxVida;
        cordura = maxCordura;
        fichas = 0;
        ActualizarUI();
    }

    void Update()
    {
        ActualizarUI();
    }

    void ActualizarUI()
    {
        // Actualizar textos en pantalla
        if (vidaText != null)
            vidaText.text = "Vida: " + vida;

        if (corduraText != null)
            corduraText.text = "Cordura: " + cordura;

        if (fichasText != null)
            fichasText.text = "Fichas: " + fichas;
    }

    public bool VidaLlena()
    {
        return vida >= maxVida;
    }

    public bool CorduraLlena()
    {
        return cordura >= maxCordura;
    }

    // Métodos para modificar valores
    public void CambiarVida(int cantidad)
    {
        vida += cantidad;
        vida = Mathf.Clamp(vida, 0, maxVida);
    }

    public void CambiarCordura(int cantidad)
    {
        cordura += cantidad;
        cordura = Mathf.Clamp(cordura, 0, maxCordura);
    }

    public void CambiarFichas(int cantidad)
    {
        fichas += cantidad;
        if (fichas < 0) fichas = 0;
    }

    // Método para mostrar mensajes temporales
    public void MostrarMensaje(string mensaje)
    {
        if (mensajeTexto != null)
        {
            mensajeTexto.text = mensaje;
            mensajeTexto.gameObject.SetActive(true);
            Invoke("OcultarMensaje", tiempoMensaje);
        }
    }

    void OcultarMensaje()
    {
        if (mensajeTexto != null)
            mensajeTexto.gameObject.SetActive(false);
    }
}