using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapaPistas : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject canvasPista1;
    public TextMeshProUGUI textoPista1;

    [Header("Contenido Pista 1")]
    [TextArea(3, 10)]
    public string contenidoPista1 = "PISTA 1: Busca detrás del árbol grande en el jardín principal. Allí encontrarás la siguiente pista.";

    private bool jugadorCerca = false;
    private PlayerUI playerUI;

    void Start()
    {
        playerUI = FindObjectOfType<PlayerUI>();

        if (canvasPista1 != null)
            canvasPista1.SetActive(false);
    }

    void Update()
    {
        // Permite abrir/cerrar la pista con la tecla 1
        if (Input.GetKeyDown(KeyCode.Alpha1) && jugadorCerca)
        {
            TogglePista1();
        }

        // Cierra la pista con la tecla ESC
        if (Input.GetKeyDown(KeyCode.Escape) && canvasPista1 != null && canvasPista1.activeSelf)
        {
            CerrarPista1();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            Debug.Log("Mapa encontrado - Presiona 1 para ver la Pista 1");

            if (playerUI != null)
            {
                // Espacio reservado para integración con la UI del jugador
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            CerrarPista1();
        }
    }

    void TogglePista1()
    {
        if (canvasPista1 != null)
        {
            bool estadoActual = canvasPista1.activeSelf;

            if (!estadoActual)
            {
                // Activa la visualización de la pista
                canvasPista1.SetActive(true);
                if (textoPista1 != null)
                    textoPista1.text = contenidoPista1;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Debug.Log("Pista 1 abierta");
            }
            else
            {
                // Oculta la pista
                CerrarPista1();
            }
        }
    }

    void CerrarPista1()
    {
        if (canvasPista1 != null)
        {
            canvasPista1.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void CambiarContenidoPista1(string nuevoContenido)
    {
        contenidoPista1 = nuevoContenido;
        if (textoPista1 != null && canvasPista1.activeSelf)
            textoPista1.text = contenidoPista1;
    }
}
