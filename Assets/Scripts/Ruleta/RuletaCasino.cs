using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuletaCasino : MonoBehaviour
{
    [Header("UI Ruleta")]
    public GameObject canvasRuleta;
    public TMP_InputField inputNumero;
    public TextMeshProUGUI numeroGirando;
    public TextMeshProUGUI fichasActualesText;
    public TextMeshProUGUI vidaActualText;
    public TextMeshProUGUI corduraActualText;
    public TextMeshProUGUI resultadoText;
    public TextMeshProUGUI mensajeText;
    public TextMeshProUGUI instruccionesText;

    [Header("Configuración")]
    public float rangoInteraccion = 3f;
    public KeyCode teclaInteraccion = KeyCode.L;
    public KeyCode teclaApostarFichas = KeyCode.T;
    public KeyCode teclaApostarVida = KeyCode.U;

    [Header("Costos de Apuesta")]
    public int costoFichas = 5;
    public int costoVida = 20;
    public int costoCordura = 10;

    [Header("Premios - Fichas")]
    public int premioBaseFichas = 50;
    public int multiplicadorNumeroExacto = 36;
    public int premioColor = 10;
    public int premioParImpar = 15;
    public int premioDocena = 20;

    [Header("Premios - Vida/Cordura")]
    public int premioBaseVida = 50;
    public int premioBaseCordura = 30;
    public int premioBaseFichasVida = 25;

    [Header("Animación - GIRO RÁPIDO")]
    public float duracionGiro = 2.5f;
    public float velocidadInicial = 0.02f;
    public float velocidadFinal = 0.1f;

    private bool jugadorCerca = false;
    private bool girando = false;
    private PlayerUI playerUI;
    private int numeroApostado = -1;
    private int[] numerosRuleta = { 0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27, 13, 36, 11, 30, 8, 23, 10, 5, 24, 16, 33, 1, 20, 14, 31, 9, 22, 18, 29, 7, 28, 12, 35, 3, 26 };

    void Start()
    {
        playerUI = FindObjectOfType<PlayerUI>();
        if (canvasRuleta != null)
            canvasRuleta.SetActive(false);
        ConfigurarUI();
    }

    void ConfigurarUI()
    {
        if (inputNumero != null)
        {
            inputNumero.characterLimit = 2;
            inputNumero.contentType = TMP_InputField.ContentType.IntegerNumber;
            inputNumero.onValueChanged.AddListener(ValidarNumero);
        }
    }

    void ValidarNumero(string valor)
    {
        if (string.IsNullOrEmpty(valor)) return;
        if (int.TryParse(valor, out int numero))
        {
            if (numero < 0) inputNumero.text = "0";
            if (numero > 36) inputNumero.text = "36";
        }
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(teclaInteraccion))
            ToggleRuleta();

        if (canvasRuleta != null && canvasRuleta.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            CerrarRuleta();

        if (canvasRuleta != null && canvasRuleta.activeSelf && !girando)
        {
            if (Input.GetKeyDown(teclaApostarFichas))
                IniciarApuesta(TipoApuesta.Fichas);
            if (Input.GetKeyDown(teclaApostarVida))
                IniciarApuesta(TipoApuesta.Vida);
        }

        if (canvasRuleta != null && canvasRuleta.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (playerUI == null)
                playerUI = FindObjectOfType<PlayerUI>();
            MostrarMensaje("Presiona [L] para jugar a la ruleta");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (canvasRuleta != null && canvasRuleta.activeSelf)
                CerrarRuleta();
            OcultarMensaje();
        }
    }

    void ToggleRuleta()
    {
        if (canvasRuleta == null) return;
        if (canvasRuleta.activeSelf)
            CerrarRuleta();
        else
            AbrirRuleta();
    }

    void AbrirRuleta()
    {
        canvasRuleta.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        InputManager.RegistrarCanvas(canvasRuleta);
        if (playerUI != null)
            playerUI.ActivarModoUI();
        ResetearUI();
        ActualizarEstadisticas();

        if (inputNumero != null)
        {
            inputNumero.text = "";
            inputNumero.Select();
            inputNumero.ActivateInputField();
        }
        OcultarMensaje();
    }

    void CerrarRuleta()
    {
        if (canvasRuleta != null)
            InputManager.DesregistrarCanvas(canvasRuleta);
        canvasRuleta.SetActive(false);
        if (playerUI != null)
            playerUI.DesactivarModoUI();
        if (jugadorCerca)
            MostrarMensaje("Presiona [L] para jugar a la ruleta");
    }

    void ResetearUI()
    {
        if (inputNumero != null)
        {
            inputNumero.text = "";
            inputNumero.interactable = true;
        }
        if (numeroGirando != null)
        {
            numeroGirando.text = "?";
            numeroGirando.color = Color.white;
            numeroGirando.fontSize = 80;
        }
        if (resultadoText != null)
            resultadoText.text = "";
        if (mensajeText != null)
            mensajeText.text = "";
        if (instruccionesText != null)
            instruccionesText.text = "Escribe un número (0-36)\n\n[T] Apostar 5 Fichas\n[U] Apostar 20 Vida + 10 Cordura\n\n[ESC] Cerrar";
    }

    void ActualizarEstadisticas()
    {
        if (playerUI == null) return;
        if (fichasActualesText != null)
            fichasActualesText.text = $" Fichas: {playerUI.fichas}";
        if (vidaActualText != null)
            vidaActualText.text = $"❤️ Vida: {playerUI.vida}/{playerUI.maxVida}";
        if (corduraActualText != null)
            corduraActualText.text = $"🧠 Cordura: {playerUI.cordura}/{playerUI.maxCordura}";
    }

    enum TipoApuesta { Fichas, Vida }

    void IniciarApuesta(TipoApuesta tipo)
    {
        if (girando)
        {
            MostrarMensajeRuleta("⚠️ La ruleta ya está girando");
            return;
        }

        if (inputNumero == null || string.IsNullOrEmpty(inputNumero.text))
        {
            MostrarMensajeRuleta("❌ Debes escribir un número del 0 al 36");
            return;
        }

        if (!int.TryParse(inputNumero.text, out numeroApostado) || numeroApostado < 0 || numeroApostado > 36)
        {
            MostrarMensajeRuleta("❌ Número inválido (0-36)");
            return;
        }

        if (playerUI == null)
        {
            Debug.LogError("❌ PlayerUI no encontrado");
            return;
        }

        if (tipo == TipoApuesta.Fichas)
        {
            if (playerUI.fichas < costoFichas)
            {
                MostrarMensajeRuleta($"❌ Necesitas {costoFichas} fichas");
                return;
            }
            playerUI.CambiarFichas(-costoFichas);
        }
        else
        {
            if (playerUI.vida < costoVida)
            {
                MostrarMensajeRuleta($"❌ Necesitas {costoVida} de vida");
                return;
            }
            if (playerUI.cordura < costoCordura)
            {
                MostrarMensajeRuleta($"❌ Necesitas {costoCordura} de cordura");
                return;
            }
            playerUI.CambiarVida(-costoVida);
            playerUI.CambiarCordura(-costoCordura);
        }

        ActualizarEstadisticas();
        StartCoroutine(AnimarGiro(tipo));
    }

    IEnumerator AnimarGiro(TipoApuesta tipoApuesta)
    {
        girando = true;
        if (inputNumero != null) inputNumero.interactable = false;
        MostrarMensajeRuleta($"🎰 Girando... Apostaste al {numeroApostado}");

        // CORRECCIÓN: Usar UnityEngine.Random explícitamente
        int numeroGanador = numerosRuleta[UnityEngine.Random.Range(0, numerosRuleta.Length)];
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracionGiro)
        {
            tiempoTranscurrido += Time.deltaTime;
            float progreso = tiempoTranscurrido / duracionGiro;
            float velocidad = Mathf.Lerp(velocidadInicial, velocidadFinal, progreso);
            yield return new WaitForSeconds(velocidad);

            // CORRECCIÓN: Usar UnityEngine.Random explícitamente
            int numeroRandom = numerosRuleta[UnityEngine.Random.Range(0, numerosRuleta.Length)];
            if (numeroGirando != null)
            {
                numeroGirando.text = numeroRandom.ToString();
                numeroGirando.color = ObtenerColorNumero(numeroRandom);
            }
        }

        if (numeroGirando != null)
        {
            numeroGirando.text = numeroGanador.ToString();
            numeroGirando.color = ObtenerColorNumero(numeroGanador);
            numeroGirando.fontSize = 120;
        }

        yield return new WaitForSeconds(0.3f);
        CalcularPremio(tipoApuesta, numeroGanador);
        ActualizarEstadisticas();
        yield return new WaitForSeconds(2f);

        ResetearUI();
        if (inputNumero != null)
        {
            inputNumero.interactable = true;
            inputNumero.Select();
            inputNumero.ActivateInputField();
        }
        girando = false;
    }

    void CalcularPremio(TipoApuesta tipoApuesta, int numeroGanador)
    {
        bool ganoNumeroExacto = (numeroGanador == numeroApostado);
        bool mismoColor = (ObtenerColorNumero(numeroGanador) == ObtenerColorNumero(numeroApostado));
        bool mismoParImpar = (EsPar(numeroGanador) == EsPar(numeroApostado) && numeroGanador != 0 && numeroApostado != 0);
        bool mismaDocena = (ObtenerDocena(numeroGanador) == ObtenerDocena(numeroApostado) && numeroGanador != 0 && numeroApostado != 0);

        if (tipoApuesta == TipoApuesta.Fichas)
        {
            int premioTotal = 0;
            string detallePremio = "";

            if (ganoNumeroExacto)
            {
                premioTotal += costoFichas * multiplicadorNumeroExacto;
                detallePremio += $"\n🎯 Número Exacto: +{costoFichas * multiplicadorNumeroExacto}";
            }
            if (mismoColor && !ganoNumeroExacto)
            {
                premioTotal += premioColor;
                detallePremio += $"\n🎨 Mismo Color: +{premioColor}";
            }
            if (mismoParImpar && !ganoNumeroExacto)
            {
                premioTotal += premioParImpar;
                detallePremio += $"\n🔢 Par/Impar: +{premioParImpar}";
            }
            if (mismaDocena && !ganoNumeroExacto)
            {
                premioTotal += premioDocena;
                detallePremio += $"\n📦 Misma Docena: +{premioDocena}";
            }

            if (premioTotal == 0)
            {
                premioTotal = premioBaseFichas;
                detallePremio = $"\n💰 Premio Base: +{premioBaseFichas}";
                MostrarResultado($"🎁 ¡PREMIO BASE!\n\nSalió: {numeroGanador}{detallePremio}", Color.yellow);
            }
            else
            {
                MostrarResultado($"🎉 ¡GANASTE!\n\nSalió: {numeroGanador}{detallePremio}\n\n💰 TOTAL: +{premioTotal} fichas", Color.green);
            }
            playerUI.CambiarFichas(premioTotal);
        }
        else
        {
            int premioVida = 0;
            int premioCordura = 0;
            int premioFichas = 0;
            string detallePremio = "";

            if (ganoNumeroExacto)
            {
                premioVida = premioBaseVida * 2;
                premioCordura = premioBaseCordura * 2;
                premioFichas = premioBaseFichasVida * 3;
                detallePremio = $"\n🎯 NÚMERO EXACTO!";
            }
            else if (mismoColor || mismoParImpar || mismaDocena)
            {
                premioVida = premioBaseVida;
                premioCordura = premioBaseCordura;
                premioFichas = premioBaseFichasVida;
                detallePremio = $"\n⭐ PREMIO SECUNDARIO";
            }
            else
            {
                premioVida = premioBaseVida / 2;
                premioCordura = premioBaseCordura / 2;
                premioFichas = premioBaseFichasVida / 2;
                detallePremio = $"\n🎁 PREMIO BASE";
            }

            playerUI.CambiarVida(premioVida);
            playerUI.CambiarCordura(premioCordura);
            playerUI.CambiarFichas(premioFichas);
            MostrarResultado($"🎊 ¡RECOMPENSA!\n\nSalió: {numeroGanador}{detallePremio}\n\n❤️ +{premioVida} Vida\n🧠 +{premioCordura} Cordura\n💰 +{premioFichas} Fichas",
                ganoNumeroExacto ? Color.green : Color.yellow);
        }
    }

    bool EsPar(int numero)
    {
        return numero % 2 == 0;
    }

    int ObtenerDocena(int numero)
    {
        if (numero == 0) return 0;
        if (numero <= 12) return 1;
        if (numero <= 24) return 2;
        return 3;
    }

    bool EsNumeroRojo(int numero)
    {
        int[] rojos = { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };
        foreach (int rojo in rojos)
        {
            if (numero == rojo) return true;
        }
        return false;
    }

    Color ObtenerColorNumero(int numero)
    {
        if (numero == 0) return Color.green;
        return EsNumeroRojo(numero) ? Color.red : new Color(0.2f, 0.2f, 0.2f);
    }

    void MostrarResultado(string mensaje, Color color)
    {
        if (resultadoText != null)
        {
            resultadoText.text = mensaje;
            resultadoText.color = color;
        }
    }

    void MostrarMensajeRuleta(string mensaje)
    {
        if (mensajeText != null)
            mensajeText.text = mensaje;
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

    void OnDestroy()
    {
        if (canvasRuleta != null && canvasRuleta.activeSelf)
        {
            InputManager.DesregistrarCanvas(canvasRuleta);
        }
    }
}