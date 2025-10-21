using System.Collections;
using UnityEngine;

public class EnemyLanzador : MonoBehaviour
{
    [Header("Stats Lanzador")]
    public int vida = 30;
    public int dañoPelota = 25;
    public float velocidad = 3f;
    public float rangoDeteccion = 15f;
    public float rangoAtaque = 10f;
    public float cadenciaAtaque = 2f;

    [Header("Referencias")]
    public GameObject pelotaPrefab;
    public Transform puntoLanzamiento;

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
            // Perseguir al player
            Vector3 direccion = (player.position - transform.position).normalized;
            transform.position += direccion * velocidad * Time.deltaTime;
            transform.LookAt(player);

            // Atacar si está en rango
            if (distancia <= rangoAtaque && puedeAtacar)
            {
                StartCoroutine(Atacar());
            }
        }
    }

    IEnumerator Atacar()
    {
        puedeAtacar = false;

        // Lanzar pelota
        if (pelotaPrefab != null && puntoLanzamiento != null)
        {
            GameObject pelota = Instantiate(pelotaPrefab, puntoLanzamiento.position, puntoLanzamiento.rotation);
            PelotaProyectil proyectil = pelota.GetComponent<PelotaProyectil>();
            if (proyectil != null)
            {
                proyectil.daño = dañoPelota;
                proyectil.direccion = (player.position - puntoLanzamiento.position).normalized;
            }

            Debug.Log("Enemigo lanzó una pelota!");
        }

        yield return new WaitForSeconds(cadenciaAtaque);
        puedeAtacar = true;
    }

    public void RecibirDaño(int cantidad)
    {
        vidaActual -= cantidad;

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        if (spawnManager != null)
        {
            spawnManager.EnemigoDerrotado(tipoEnemigo);
        }

        Destroy(gameObject);
        Debug.Log("Enemigo Lanzador derrotado!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}