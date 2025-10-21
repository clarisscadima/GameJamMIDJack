using UnityEngine;

public class PelotaProyectil : MonoBehaviour
{
    [Header("Configuración Pelota")]
    public int daño = 25;
    public float velocidad = 8f;
    public float tiempoVida = 5f;

    public Vector3 direccion;

    void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        // Movimiento de la pelota
        transform.position += direccion * velocidad * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Aplicar daño al player
            PlayerUI playerUI = FindObjectOfType<PlayerUI>();
            if (playerUI != null)
            {
                playerUI.CambiarVida(-daño);
                Debug.Log("Pelota golpeó al player! -" + daño + " vida");
            }

            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy")) // No destruir si choca con otro enemigo
        {
            // Destruir al chocar con cualquier cosa que no sea enemigo
            Destroy(gameObject);
        }
    }
}