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
        transform.position += direccion * velocidad * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerUI playerUI = FindObjectOfType<PlayerUI>();
            if (playerUI != null)
                playerUI.CambiarVida(-daño);
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}