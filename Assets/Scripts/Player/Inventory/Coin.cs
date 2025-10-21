using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Valor de la Ficha")]
    public int valorFicha = 1;

    [Header("Efectos")]
    public GameObject particulasRecoleccion;
    public float velocidadRotacion = 100f;

    void Update()
    {
        // Rotar la ficha para que sea más visible
        transform.Rotate(0, velocidadRotacion * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RecolectarFicha(other.gameObject);
        }
    }

    void RecolectarFicha(GameObject jugador)
    {
        // Obtener el sistema de UI del jugador
        PlayerUI ui = FindObjectOfType<PlayerUI>();

        // Dar fichas al jugador
        if (ui != null)
        {
            ui.CambiarFichas(valorFicha);
        }

        // Efectos visuales
        if (particulasRecoleccion != null)
        {
            Instantiate(particulasRecoleccion, transform.position, Quaternion.identity);
        }

        // Sonido (opcional)
        // AudioSource.PlayClipAtPoint(sonidoRecoleccion, transform.position);

        // Destruir la ficha
        Destroy(gameObject);

        Debug.Log($"¡Ficha recolectada! +{valorFicha} fichas");
    }
}