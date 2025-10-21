using UnityEngine;

public class PelotaProyectil : MonoBehaviour
{
    [Header("Configuraci�n Pelota")]
    public int da�o = 25;
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
            // Aplicar da�o al player
            PlayerUI playerUI = FindObjectOfType<PlayerUI>();
            if (playerUI != null)
            {
                playerUI.CambiarVida(-da�o);
                Debug.Log("Pelota golpe� al player! -" + da�o + " vida");
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