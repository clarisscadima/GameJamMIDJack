using UnityEngine;

public class Coin : MonoBehaviour
{
    public int valorFicha = 1;
    public GameObject particulasRecoleccion;
    public float velocidadRotacion = 100f;

    void Update()
    {
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
        PlayerUI ui = FindObjectOfType<PlayerUI>();
        if (ui != null)
            ui.CambiarFichas(valorFicha);

        if (particulasRecoleccion != null)
            Instantiate(particulasRecoleccion, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}