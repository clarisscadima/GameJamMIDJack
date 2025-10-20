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

    void Start()
    {
        // Inicializar con valores por defecto
        vida = 100;
        cordura = 100;
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

    // Métodos para modificar valores
    public void CambiarVida(int cantidad)
    {
        vida += cantidad;
        vida = Mathf.Clamp(vida, 0, 100);
    }

    public void CambiarCordura(int cantidad)
    {
        cordura += cantidad;
        cordura = Mathf.Clamp(cordura, 0, 100);
    }

    public void CambiarFichas(int cantidad)
    {
        fichas += cantidad;
        if (fichas < 0) fichas = 0;
    }
}