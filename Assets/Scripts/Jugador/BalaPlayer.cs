using UnityEngine;

public class BalaPlayer : MonoBehaviour
{
    public int daño = 20;
    public float velocidad = 15f;
    public float tiempoVida = 3f;
    public Vector3 direccion;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.velocity = direccion.normalized * velocidad;
        }
        Destroy(gameObject, tiempoVida);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyLanzador enemigoLanzador = other.GetComponent<EnemyLanzador>();
            EnemyMelee enemigoMelee = other.GetComponent<EnemyMelee>();

            if (enemigoLanzador != null)
            {
                enemigoLanzador.RecibirDaño(daño);
            }
            else if (enemigoMelee != null)
            {
                enemigoMelee.RecibirDaño(daño);
            }
        }
        Destroy(gameObject);
    }
}