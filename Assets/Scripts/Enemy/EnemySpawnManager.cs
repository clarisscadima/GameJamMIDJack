using System.Collections;
using TMPro;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Prefabs de Enemigos")]
    public GameObject enemyLanzadorPrefab;
    public GameObject enemyMeleePrefab;

    [Header("Configuración Spawn")]
    public float tiempoEntreSpawn = 120f; // 2 minutos
    public int maxEnemigosPorTipo = 3;

    [Header("UI Contador")]
    public TextMeshProUGUI contadorText;

    private int enemigosLanzadoresCount = 0;
    private int enemigosMeleeCount = 0;
    private float tiempoSiguienteSpawn;

    void Start()
    {
        tiempoSiguienteSpawn = tiempoEntreSpawn;
        ActualizarUI();
        StartCoroutine(SpawnCorrutina());
    }

    void Update()
    {
        // Contador regresivo en UI
        tiempoSiguienteSpawn -= Time.deltaTime;
        ActualizarUI();

        if (tiempoSiguienteSpawn <= 0)
        {
            GenerarSpawn();
            tiempoSiguienteSpawn = tiempoEntreSpawn;
        }
    }

    IEnumerator SpawnCorrutina()
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempoEntreSpawn);
            GenerarSpawn();
        }
    }

    void GenerarSpawn()
    {
        // Alternar entre spawn de lanzador y melee
        if (enemigosLanzadoresCount <= enemigosMeleeCount && enemigosLanzadoresCount < maxEnemigosPorTipo)
        {
            SpawnEnemigoLanzador();
        }
        else if (enemigosMeleeCount < maxEnemigosPorTipo)
        {
            SpawnEnemigoMelee();
        }
    }

    void SpawnEnemigoLanzador()
    {
        if (spawnPoints.Length > 0 && enemyLanzadorPrefab != null)
        {
            // CORRECCIÓN: Usar UnityEngine.Random
            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            GameObject enemigo = Instantiate(enemyLanzadorPrefab, spawnPoint.position, spawnPoint.rotation);

            EnemyLanzador lanzador = enemigo.GetComponent<EnemyLanzador>();
            if (lanzador != null)
            {
                lanzador.spawnManager = this;
                lanzador.tipoEnemigo = TipoEnemigo.Lanzador;
            }

            enemigosLanzadoresCount++;
            Debug.Log("Enemigo Lanzador spawnedo");
        }
    }

    void SpawnEnemigoMelee()
    {
        if (spawnPoints.Length > 0 && enemyMeleePrefab != null)
        {
            // CORRECCIÓN: Usar UnityEngine.Random
            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            GameObject enemigo = Instantiate(enemyMeleePrefab, spawnPoint.position, spawnPoint.rotation);

            EnemyMelee melee = enemigo.GetComponent<EnemyMelee>();
            if (melee != null)
            {
                melee.spawnManager = this;
                melee.tipoEnemigo = TipoEnemigo.Melee;
            }

            enemigosMeleeCount++;
            Debug.Log("Enemigo Melee spawnedo");
        }
    }

    public void EnemigoDerrotado(TipoEnemigo tipo)
    {
        if (tipo == TipoEnemigo.Lanzador)
        {
            enemigosLanzadoresCount--;
        }
        else if (tipo == TipoEnemigo.Melee)
        {
            enemigosMeleeCount--;
        }

        enemigosLanzadoresCount = Mathf.Max(0, enemigosLanzadoresCount);
        enemigosMeleeCount = Mathf.Max(0, enemigosMeleeCount);
    }

    void ActualizarUI()
    {
        if (contadorText != null)
        {
            int minutos = Mathf.FloorToInt(tiempoSiguienteSpawn / 60);
            int segundos = Mathf.FloorToInt(tiempoSiguienteSpawn % 60);

            contadorText.text = $"Siguiente spawn: {minutos:00}:{segundos:00}\n" +
                               $"Lanzadores: {enemigosLanzadoresCount}\n" +
                               $"Melee: {enemigosMeleeCount}";
        }
    }
}

public enum TipoEnemigo
{
    Lanzador,
    Melee
}