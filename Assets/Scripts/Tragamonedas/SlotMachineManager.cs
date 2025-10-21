using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject slotMachineCanvas;
    public Button buttonVidaCordura;
    public Button buttonMonedas;
    public TextMeshProUGUI resultadoText;
    public TextMeshProUGUI simbolosText;
    public TextMeshProUGUI statsText;

    [Header("Referencias Visuales Máquina")]
    public GameObject modeloMaquina; // El modelo 3D de la máquina
    public Collider colliderMaquina; // El collider de la máquina

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

    private bool maquinaActiva = false;
    private bool jugando = false;
    private PlayerUI playerUI;
    private InventorySystem inventory;

    // Partes de la máquina que el jugador debe encontrar
    private int partesEncontradas = 0;
    private const int PARTES_NECESARIAS = 5;

    void Start()
    {
        playerUI = FindObjectOfType<PlayerUI>();
        inventory = FindObjectOfType<InventorySystem>();

        // Ocultar UI al inicio
        if (slotMachineCanvas != null)
            slotMachineCanvas.SetActive(false);

        // Ocultar la máquina física hasta que se encuentren todas las partes
        OcultarMaquina();
    }

    void OcultarMaquina()
    {
        // Ocultar el modelo 3D
        if (modeloMaquina != null)
            modeloMaquina.SetActive(false);

        // Desactivar el collider
        if (colliderMaquina != null)
            colliderMaquina.enabled = false;

        Debug.Log("Máquina oculta - Encuentra las 5 partes para revelarla");
    }

    void MostrarMaquina()
    {
        // Mostrar el modelo 3D
        if (modeloMaquina != null)
            modeloMaquina.SetActive(true);

        // Activar el collider
        if (colliderMaquina != null)
            colliderMaquina.enabled = true;

        Debug.Log("¡Máquina revelada! Ahora puedes acercarte y presionar O para jugar");
    }

    void Update()
    {
        // Tecla O para abrir la máquina si está cerca y activa
        if (Input.GetKeyDown(KeyCode.O) && maquinaActiva && !jugando && partesEncontradas >= PARTES_NECESARIAS)
        {
            AbrirMaquina();
        }

        // Actualizar stats en UI
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

            // Mostrar mensaje directo sin depender de PlayerUI
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

            // Ocultar mensaje directo
            if (playerUI != null && playerUI.mensajeTexto != null)
            {
                playerUI.mensajeTexto.gameObject.SetActive(false);
            }
        }
    }

    // Llamado cuando el jugador encuentra una parte
    public void EncontrarParte()
    {
        if (partesEncontradas < PARTES_NECESARIAS)
        {
            partesEncontradas++;
            Debug.Log($"Parte encontrada: {partesEncontradas}/{PARTES_NECESARIAS}");

            // Si se encontraron todas las partes, mostrar la máquina
            if (partesEncontradas >= PARTES_NECESARIAS)
            {
                MostrarMaquina();
                Debug.Log("¡Máquina tragamonedas armada! Busca la máquina para jugar.");
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
            ActualizarBotones();
            Debug.Log("Máquina abierta - UI visible");
        }
    }

    void CerrarMaquina()
    {
        if (slotMachineCanvas != null)
        {
            slotMachineCanvas.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("Máquina cerrada - UI oculta");
        }
    }

    void ActualizarBotones()
    {
        if (playerUI == null) return;

        // Verificar si puede pagar con vida/cordura
        bool puedeVidaCordura = (playerUI.vida >= costoVida && playerUI.cordura >= costoCordura);
        if (buttonVidaCordura != null)
            buttonVidaCordura.interactable = puedeVidaCordura && !jugando;

        // Verificar si puede pagar con monedas
        bool puedeMonedas = (playerUI.fichas >= costoMonedas);
        if (buttonMonedas != null)
            buttonMonedas.interactable = puedeMonedas && !jugando;

        if (resultadoText != null)
            resultadoText.text = "Elige tu apuesta...";
    }

    // BOTÓN 1: Pagar con Vida y Cordura
    public void JugarConVidaCordura()
    {
        if (jugando || playerUI == null) return;

        // Verificar que tenga suficientes recursos
        if (playerUI.vida < costoVida || playerUI.cordura < costoCordura)
        {
            Debug.Log("No tienes suficiente vida o cordura");
            return;
        }

        // Quitar recursos
        playerUI.CambiarVida(-costoVida);
        playerUI.CambiarCordura(-costoCordura);

        StartCoroutine(GirarRodillos());
    }

    // BOTÓN 2: Pagar con Monedas
    public void JugarConMonedas()
    {
        if (jugando || playerUI == null) return;

        // Verificar que tenga suficientes monedas
        if (playerUI.fichas < costoMonedas)
        {
            Debug.Log("No tienes suficientes monedas");
            return;
        }

        // Quitar monedas
        playerUI.CambiarFichas(-costoMonedas);

        StartCoroutine(GirarRodillos());
    }

    IEnumerator GirarRodillos()
    {
        jugando = true;
        ActualizarBotones();

        if (resultadoText != null)
            resultadoText.text = "Girando...";

        // Animación de giro
        for (int i = 0; i < 15; i++)
        {
            if (simbolosText != null)
            {
                string simboloRandom = simbolos[UnityEngine.Random.Range(0, simbolos.Length)];
                simbolosText.text = simboloRandom + " | ? | ?";
            }
            yield return new WaitForSeconds(0.08f);
        }

        // Resultado final
        string simbolo1 = simbolos[UnityEngine.Random.Range(0, simbolos.Length)];
        string simbolo2 = simbolos[UnityEngine.Random.Range(0, simbolos.Length)];
        string simbolo3 = simbolos[UnityEngine.Random.Range(0, simbolos.Length)];

        if (simbolosText != null)
            simbolosText.text = $"{simbolo1} | {simbolo2} | {simbolo3}";

        // Verificar combinaciones ganadoras
        yield return new WaitForSeconds(1f);
        VerificarResultado(simbolo1, simbolo2, simbolo3);

        jugando = false;
        ActualizarBotones();
    }

    void VerificarResultado(string s1, string s2, string s3)
    {
        if (resultadoText == null || playerUI == null) return;

        // JACKPOT - Mapa para siguiente piso (777)
        if (s1 == "7" && s2 == "7" && s3 == "7")
        {
            resultadoText.text = "JACKPOT! MAPA DESBLOQUEADO";
            DarMapaSiguientePiso();
        }
        // Premio mayor - Vida y Cordura (AAA)
        else if (s1 == "A" && s2 == "A" && s3 == "A")
        {
            resultadoText.text = "PREMIO MAYOR! +Vida y +Cordura";
            playerUI.CambiarVida(vidaGanada);
            playerUI.CambiarCordura(corduraGanada);
        }
        // Tres símbolos iguales (BBB, CCC, etc.)
        else if (s1 == s2 && s2 == s3)
        {
            resultadoText.text = "TRES IGUALES! +Monedas";
            playerUI.CambiarFichas(monedasGanadas);
        }
        // Dos símbolos iguales
        else if (s1 == s2 || s2 == s3 || s1 == s3)
        {
            resultadoText.text = "DOS IGUALES! Pequeña recompensa";
            playerUI.CambiarFichas(monedasGanadas / 2);
        }
        // Perdió
        else
        {
            resultadoText.text = "Sin premio. Intenta otra vez";
        }
    }

    void DarMapaSiguientePiso()
    {
        if (mapaPrefab != null)
        {
            Vector3 posicion = transform.position + Vector3.forward * 2f;
            Instantiate(mapaPrefab, posicion, Quaternion.identity);
            Debug.Log("¡Mapa para siguiente piso generado! Busca el mapa y presiona 1 para ver la Pista 1");
        }
        else
        {
            Debug.LogError("MapaPrefab no asignado en el inspector");
        }
    }

    // Para cerrar la máquina
    public void CerrarUI()
    {
        CerrarMaquina();
    }
}