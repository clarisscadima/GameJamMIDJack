using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    [Header("Stats Melee")]
    public int vida = 50;
    public int dañoGolpe = 15;
    public float velocidad = 2f;
    public float rangoDeteccion = 12f;
    public float rangoAtaque = 2f;
    public float cadenciaAtaque = 1.5f;
    public EnemySpawnManager spawnManager;
    public TipoEnemigo tipoEnemigo;

    private Transform player;
    private bool puedeAtacar = true;
    private int vidaActual;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        vidaActual = vida;
    }

    void Update()
    {
        if (player == null) return;
        float distancia = Vector3.Distance(transform.position, player.position);

        if (distancia <= rangoDeteccion)
        {
            Vector3 direccion = (player.position - transform.position).normalized;
            transform.position += direccion * velocidad * Time.deltaTime;
            transform.LookAt(player);

            if (distancia <= rangoAtaque && puedeAtacar)
                Atacar();
        }
    }

    void Atacar()
    {
        puedeAtacar = false;
        PlayerUI playerUI = FindObjectOfType<PlayerUI>();
        if (playerUI != null)
            playerUI.CambiarVida(-dañoGolpe);
        Invoke("ResetearAtaque", cadenciaAtaque);
    }

    void ResetearAtaque()
    {
        puedeAtacar = true;
    }

    public void RecibirDaño(int cantidad)
    {
        vidaActual -= cantidad;
        if (vidaActual <= 0)
            Morir();
    }

    void Morir()
    {
        if (spawnManager != null)
            spawnManager.EnemigoDerrotado(tipoEnemigo);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}