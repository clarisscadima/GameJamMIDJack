using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Textos UI - TextMesh Pro")]
    public TextMeshProUGUI vidaText;
    public TextMeshProUGUI corduraText;
    public TextMeshProUGUI fichasText;

    [Header("Estadísticas Player")]
    public int vida = 100;
    public int cordura = 100;
    public int fichas = 0;

    [Header("Límites")]
    public int maxVida = 100;
    public int maxCordura = 100;

    [Header("Sistema de Mensajes Temporales")]
    public TextMeshProUGUI mensajeTexto;
    public float tiempoMensaje = 3f;

    private bool mostrandoMensaje = false;
    private System.Collections.Generic.Queue<string> colaMensajes = new System.Collections.Generic.Queue<string>();
    private bool uiActiva = false;
    private PlayerShooting playerShooting;
    public static PlayerUI Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        vida = maxVida;
        cordura = maxCordura;
        fichas = 0;
        ActualizarUI();

        playerShooting = FindObjectOfType<PlayerShooting>();
        if (mensajeTexto != null)
            mensajeTexto.gameObject.SetActive(false);
    }

    void Update()
    {
        ActualizarUI();
        ProcesarColaMensajes();
    }

    void ActualizarUI()
    {
        if (vidaText != null)
            vidaText.text = $"Vida: {vida}/{maxVida}";
        if (corduraText != null)
            corduraText.text = $"Cordura: {cordura}/{maxCordura}";
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

    public void CambiarVida(int cantidad)
    {
        vida += cantidad;
        vida = Mathf.Clamp(vida, 0, maxVida);
        if (cantidad > 0)
            MostrarMensaje($"+{cantidad} Vida");
        else if (cantidad < 0)
            MostrarMensaje($"{cantidad} Vida");
    }

    public void CambiarCordura(int cantidad)
    {
        cordura += cantidad;
        cordura = Mathf.Clamp(cordura, 0, maxCordura);
        if (cantidad > 0)
            MostrarMensaje($"+{cantidad} Cordura");
        else if (cantidad < 0)
            MostrarMensaje($"{cantidad} Cordura");
    }

    public void CambiarFichas(int cantidad)
    {
        fichas += cantidad;
        if (fichas < 0) fichas = 0;
        if (cantidad > 0)
            MostrarMensaje($"+{cantidad} Fichas");
        else if (cantidad < 0)
            MostrarMensaje($"{cantidad} Fichas");
    }

    public void MostrarMensaje(string mensaje, float duracion = -1)
    {
        if (mensajeTexto == null) return;

        float tiempoReal = duracion > 0 ? duracion : tiempoMensaje;
        if (mostrandoMensaje)
        {
            colaMensajes.Enqueue(mensaje);
            return;
        }
        MostrarMensajeInmediato(mensaje, tiempoReal);
    }

    private void MostrarMensajeInmediato(string mensaje, float duracion)
    {
        mostrandoMensaje = true;
        mensajeTexto.text = mensaje;
        mensajeTexto.gameObject.SetActive(true);
        CancelInvoke("OcultarMensaje");
        Invoke("OcultarMensaje", duracion);
    }

    private void ProcesarColaMensajes()
    {
        if (!mostrandoMensaje && colaMensajes.Count > 0)
        {
            string siguienteMensaje = colaMensajes.Dequeue();
            MostrarMensajeInmediato(siguienteMensaje, tiempoMensaje);
        }
    }

    private void OcultarMensaje()
    {
        if (mensajeTexto != null)
            mensajeTexto.gameObject.SetActive(false);
        mostrandoMensaje = false;
    }

    public void LimpiarColaMensajes()
    {
        colaMensajes.Clear();
    }

    public void ActivarModoUI()
    {
        uiActiva = true;
        if (playerShooting != null)
            playerShooting.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void DesactivarModoUI()
    {
        uiActiva = false;
        if (playerShooting != null)
            playerShooting.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool HayUIActiva()
    {
        return uiActiva;
    }
}