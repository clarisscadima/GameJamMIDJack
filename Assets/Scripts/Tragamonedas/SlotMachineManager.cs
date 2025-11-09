using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject slotMachineCanvas;
    public TextMeshProUGUI resultadoText;
    public TextMeshProUGUI simbolosText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI instruccionesText;

    [Header("Referencias Visuales Máquina")]
    public GameObject modeloMaquina;
    public Collider colliderMaquina;

    [Header("Costos")]
    public int costoVida = 10;
    public int costoCordura = 10;
    public int costoMonedas = 2;

    [Header("Recompensas")]
    public GameObject mapaPrefab;
    public int vidaGanada = 20;
    public int corduraGanada = 20;
    public int monedasGanadas = 10;

    [Header("Símbolos")]
    public string[] simbolos = { "A", "B", "C", "D", "E", "F", "7", "X" };

    [Header("Controles")]
    public KeyCode teclaApostarVida = KeyCode.Y;
    public KeyCode teclaApostarMonedas = KeyCode.H;

    private bool maquinaActiva = false;
    private bool jugando = false;
    private PlayerUI playerUI;
    private InventorySystem inventory;
    private int partesEncontradas = 0;
    private const int PARTES_NECESARIAS = 5;

    void Start()
    {
        playerUI = FindObjectOfType<PlayerUI>();
        inventory = FindObjectOfType<InventorySystem>();

        if (slotMachineCanvas != null)
            slotMachineCanvas.SetActive(false);

        OcultarMaquina();
    }

    void OcultarMaquina()
    {
        if (modeloMaquina != null)
            modeloMaquina.SetActive(false);
        if (colliderMaquina != null)
            colliderMaquina.enabled = false;
    }

    void MostrarMaquina()
    {
        if (modeloMaquina != null)
            modeloMaquina.SetActive(true);
        if (colliderMaquina != null)
            colliderMaquina.enabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && maquinaActiva && !jugando && partesEncontradas >= PARTES_NECESARIAS)
        {
            AbrirMaquina();
        }

        if (slotMachineCanvas != null && slotMachineCanvas.activeSelf && !jugando)
        {
            if (Input.GetKeyDown(teclaApostarVida))
            {
                JugarConVidaCordura();
            }
            if (Input.GetKeyDown(teclaApostarMonedas))
            {
                JugarConMonedas();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CerrarMaquina();
            }
        }

        if (statsText != null && playerUI != null)
        {
            statsText.text = $"Vida: {playerUI.vida}\nCordura: {playerUI.cordura}\nFichas: {playerUI.fichas}";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && partesEncontradas >= PARTES_NECESARIAS)
        {
            maquinaActiva = true;
            if (playerUI != null && playerUI.mensajeTexto != null)
            {
                playerUI.mensajeTexto.text = "Presiona O para jugar";
                playerUI.mensajeTexto.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            maquinaActiva = false;
            CerrarMaquina();
            if (playerUI != null && playerUI.mensajeTexto != null)
            {
                playerUI.mensajeTexto.gameObject.SetActive(false);
            }
        }
    }

    public void EncontrarParte()
    {
        if (partesEncontradas < PARTES_NECESARIAS)
        {
            partesEncontradas++;
            if (partesEncontradas >= PARTES_NECESARIAS)
            {
                MostrarMaquina();
            }
        }
    }

    void AbrirMaquina()
    {
        if (slotMachineCanvas != null)
        {
            slotMachineCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ActualizarInstrucciones();
        }
    }

    void CerrarMaquina()
    {
        if (slotMachineCanvas != null)
        {
            slotMachineCanvas.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void ActualizarInstrucciones()
    {
        if (instruccionesText != null)
        {
            instruccionesText.text = $"<b>TRAGAMONEDAS - CONTROLES</b>\n\n" +
                                   $"[Y] Apostar con Vida/Cordura\n" +
                                   $" -{costoVida} Vida, -{costoCordura} Cordura\n\n" +
                                   $"[H] Apostar con Monedas\n" +
                                   $" -{costoMonedas} Monedas\n\n" +
                                   $"[ESC] Salir\n\n" +
                                   $"<size=80%>Encuentra 3 símbolos iguales para ganar!</size>";
        }
        if (resultadoText != null)
            resultadoText.text = "Elige tu apuesta con Y o H...";
    }

    public void JugarConVidaCordura()
    {
        if (jugando || playerUI == null) return;

        if (playerUI.vida < costoVida || playerUI.cordura < costoCordura)
        {
            if (resultadoText != null)
                resultadoText.text = "❌ No tienes suficiente Vida/Cordura";
            return;
        }

        playerUI.CambiarVida(-costoVida);
        playerUI.CambiarCordura(-costoCordura);

        if (resultadoText != null)
            resultadoText.text = $"Apostando {costoVida} Vida y {costoCordura} Cordura...";

        StartCoroutine(GirarRodillos());
    }

    public void JugarConMonedas()
    {
        if (jugando || playerUI == null) return;

        if (playerUI.fichas < costoMonedas)
        {
            if (resultadoText != null)
                resultadoText.text = "❌ No tienes suficientes Monedas";
            return;
        }

        playerUI.CambiarFichas(-costoMonedas);

        if (resultadoText != null)
            resultadoText.text = $"Apostando {costoMonedas} Monedas...";

        StartCoroutine(GirarRodillos());
    }

    IEnumerator GirarRodillos()
    {
        jugando = true;
        if (instruccionesText != null)
            instruccionesText.text = "<b>🎰 GIRANDO...</b>";

        for (int i = 0; i < 15; i++)
        {
            if (simbolosText != null)
            {
                string simboloRandom1 = simbolos[UnityEngine.Random.Range(0, simbolos.Length)];
                string simboloRandom2 = simbolos[UnityEngine.Random.Range(0, simbolos.Length)];
                string simboloRandom3 = simbolos[UnityEngine.Random.Range(0, simbolos.Length)];
                simbolosText.text = $"{simboloRandom1} | {simboloRandom2} | {simboloRandom3}";
            }
            yield return new WaitForSeconds(0.08f);
        }

        string simbolo1 = simbolos[UnityEngine.Random.Range(0, simbolos.Length)];
        string simbolo2 = simbolos[UnityEngine.Random.Range(0, simbolos.Length)];
        string simbolo3 = simbolos[UnityEngine.Random.Range(0, simbolos.Length)];

        if (simbolosText != null)
            simbolosText.text = $"{simbolo1} | {simbolo2} | {simbolo3}";

        yield return new WaitForSeconds(1f);
        VerificarResultado(simbolo1, simbolo2, simbolo3);
        jugando = false;
        ActualizarInstrucciones();
    }

    void VerificarResultado(string s1, string s2, string s3)
    {
        if (resultadoText == null || playerUI == null) return;

        if (s1 == "7" && s2 == "7" && s3 == "7")
        {
            resultadoText.text = "🎊 ¡JACKPOT! 🎊\n¡MAPA DESBLOQUEADO!";
            DarMapaSiguientePiso();
        }
        else if (s1 == "A" && s2 == "A" && s3 == "A")
        {
            resultadoText.text = "🏆 ¡PREMIO MAYOR!\n+Vida y +Cordura";
            playerUI.CambiarVida(vidaGanada);
            playerUI.CambiarCordura(corduraGanada);
        }
        else if (s1 == s2 && s2 == s3)
        {
            resultadoText.text = "🎉 ¡TRES IGUALES!\n+Monedas";
            playerUI.CambiarFichas(monedasGanadas);
        }
        else if (s1 == s2 || s2 == s3 || s1 == s3)
        {
            resultadoText.text = "⭐ ¡DOS IGUALES!\nPequeña recompensa";
            playerUI.CambiarFichas(monedasGanadas / 2);
        }
        else
        {
            resultadoText.text = "💔 Sin premio\nIntenta otra vez";
        }
    }

    void DarMapaSiguientePiso()
    {
        if (mapaPrefab != null)
        {
            Vector3 posicion = transform.position + Vector3.forward * 2f;
            Instantiate(mapaPrefab, posicion, Quaternion.identity);
        }
        else
        {
            Debug.LogError("MapaPrefab no asignado en el inspector");
        }
    }

    public void CerrarUI()
    {
        CerrarMaquina();
    }
}