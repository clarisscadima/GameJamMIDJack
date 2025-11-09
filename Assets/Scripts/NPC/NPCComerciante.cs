using TMPro;
using UnityEngine;

public class NPCComerciante : MonoBehaviour
{
    [Header("UI del Comerciante")]
    public GameObject canvasComerciante;
    public TextMeshProUGUI dialogoText;

    [Header("Configuración")]
    public float rangoInteraccion = 3f;
    public KeyCode teclaInteraccion = KeyCode.P;

    private bool jugadorCerca = false;
    private PlayerUI playerUI;
    private InventorySystem inventario;

    void Start()
    {
        playerUI = FindObjectOfType<PlayerUI>();
        inventario = FindObjectOfType<InventorySystem>();
        if (canvasComerciante != null)
            canvasComerciante.SetActive(false);
    }

    void Update()
    {
        if (canvasComerciante != null && canvasComerciante.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (jugadorCerca && Input.GetKeyDown(teclaInteraccion))
            ToggleTienda();

        if (canvasComerciante != null && canvasComerciante.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            CerrarTienda();

        if (canvasComerciante != null && canvasComerciante.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) ComprarPocionVida();
            if (Input.GetKeyDown(KeyCode.Alpha2)) ComprarElixirCordura();
            if (Input.GetKeyDown(KeyCode.Alpha3)) ComprarPaqueteBalas();
            if (Input.GetKeyDown(KeyCode.Alpha4)) ComprarKitCompleto();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (playerUI == null) playerUI = FindObjectOfType<PlayerUI>();
            if (inventario == null) inventario = FindObjectOfType<InventorySystem>();
            MostrarMensaje($"Presiona [{teclaInteraccion}] para hablar con el comerciante");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (canvasComerciante != null && canvasComerciante.activeSelf)
                CerrarTienda();
            OcultarMensaje();
        }
    }

    void ToggleTienda()
    {
        if (canvasComerciante == null) return;
        if (canvasComerciante.activeSelf)
            CerrarTienda();
        else
            AbrirTienda();
    }

    void AbrirTienda()
    {
        canvasComerciante.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (playerUI != null)
            playerUI.ActivarModoUI();
        ActualizarDialogo();
        OcultarMensaje();
    }

    void CerrarTienda()
    {
        canvasComerciante.SetActive(false);
        if (playerUI != null)
            playerUI.DesactivarModoUI();
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (jugadorCerca)
            MostrarMensaje($"Presiona [{teclaInteraccion}] para hablar con el comerciante");
    }

    void ActualizarDialogo()
    {
        if (dialogoText != null && playerUI != null)
        {
            dialogoText.text = $"BIENVENIDO, VIAJERO!\nTus fichas: {playerUI.fichas}\n\n" +
                             "========== TIENDA ==========\n\n" +
                             "Teclas [1-4] o botones | [ESC] salir";
        }
    }

    public void ComprarPocionVida()
    {
        ComprarProducto(1, 25, 0, 0, "Pocion de Vida", "+25 Vida");
        ActualizarDialogo();
    }

    public void ComprarElixirCordura()
    {
        ComprarProducto(2, 0, 20, 0, "Elixir de Cordura", "+20 Cordura");
        ActualizarDialogo();
    }

    public void ComprarPaqueteBalas()
    {
        ComprarProducto(1, 0, 0, 6, "Paquete de Balas", "+6 Balas");
        ActualizarDialogo();
    }

    public void ComprarKitCompleto()
    {
        ComprarProducto(3, 15, 15, 4, "Kit Completo", "+15 Vida, +15 Cordura, +4 Balas");
        ActualizarDialogo();
    }

    void ComprarProducto(int precio, int vida, int cordura, int balas, string nombre, string beneficios)
    {
        if (playerUI == null) return;
        if (inventario == null) return;

        if (playerUI.fichas < precio)
        {
            if (dialogoText != null)
                dialogoText.text = $"NO TIENES SUFICIENTES FICHAS\n\n" +
                                 $"Necesitas: {precio} fichas\n" +
                                 $"Tienes: {playerUI.fichas} fichas\n\n" +
                                 "Consigue mas fichas y vuelve!";
            return;
        }

        playerUI.CambiarFichas(-precio);
        if (vida > 0) playerUI.CambiarVida(vida);
        if (cordura > 0) playerUI.CambiarCordura(cordura);
        if (balas > 0) inventario.AgregarItem("Balas", balas);

        if (dialogoText != null)
        {
            dialogoText.text = $"COMPRA EXITOSA!\n\n" +
                             $"Has adquirido:\n{nombre}\n{beneficios}\n\n" +
                             $"Fichas restantes: {playerUI.fichas}\n\n" +
                             "Presiona cualquier boton para continuar\n" +
                             "o [ESC] para salir";
        }
    }

    void MostrarMensaje(string mensaje)
    {
        if (playerUI != null && playerUI.mensajeTexto != null)
        {
            playerUI.mensajeTexto.text = mensaje;
            playerUI.mensajeTexto.gameObject.SetActive(true);
        }
    }

    void OcultarMensaje()
    {
        if (playerUI != null && playerUI.mensajeTexto != null)
        {
            playerUI.mensajeTexto.gameObject.SetActive(false);
        }
    }
}