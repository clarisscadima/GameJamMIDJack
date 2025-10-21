using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Recursos del Cofre")]
    public int vidaParaDar = 25;
    public int balasParaDar = 6;
    public int fichasParaDar = 5;

    [Header("Efectos Visuales")]
    public GameObject heartEffect;
    public GameObject bulletEffect;
    public GameObject coinEffect;

    public float explosionForce = 12f;

    private bool yaRecolectado = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !yaRecolectado)
        {
            RecolectarCofre();
        }
    }

    void RecolectarCofre()
    {
        yaRecolectado = true;

        // 1. DAR RECURSOS AL JUGADOR
        DarRecursosAlJugador();

        // 2. EXPULSAR OBJETOS VISUALES
        ExpulsarObjetosVisuales();

        // 3. DESAPARECER COFRE
        DesaparecerCofre();
    }

    void DarRecursosAlJugador()
    {
        // Buscar los sistemas del jugador
        PlayerUI uiSystem = FindObjectOfType<PlayerUI>();
        InventorySystem inventorySystem = FindObjectOfType<InventorySystem>();

        // Dar VIDA
        if (uiSystem != null)
        {
            uiSystem.CambiarVida(vidaParaDar);
            Debug.Log("✅ +" + vidaParaDar + " VIDA");
        }

        // Dar BALAS
        if (inventorySystem != null)
        {
            inventorySystem.AgregarItem("Balas", balasParaDar);
            Debug.Log("✅ +" + balasParaDar + " BALAS");
        }

        // Dar FICHAS
        if (uiSystem != null)
        {
            uiSystem.CambiarFichas(fichasParaDar);
            Debug.Log("✅ +" + fichasParaDar + " FICHAS");
        }
    }

    void ExpulsarObjetosVisuales()
    {
        Vector3 posicionCofre = transform.position;

        // Expulsar CORAZONES (vida)
        for (int i = 0; i < 3; i++)
        {
            if (heartEffect != null)
                CrearObjetoVolador(heartEffect, posicionCofre);
        }

        // Expulsar BALAS
        for (int i = 0; i < 2; i++)
        {
            if (bulletEffect != null)
                CrearObjetoVolador(bulletEffect, posicionCofre);
        }

        // Expulsar MONEDAS (fichas)
        for (int i = 0; i < 4; i++)
        {
            if (coinEffect != null)
                CrearObjetoVolador(coinEffect, posicionCofre);
        }
    }

    void CrearObjetoVolador(GameObject prefab, Vector3 posicion)
    {
        GameObject obj = Instantiate(prefab, posicion, UnityEngine.Random.rotation);

        // Asegurar que tenga Rigidbody
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
            rb = obj.AddComponent<Rigidbody>();

        // Fuerza de expulsión (usando UnityEngine.Random)
        Vector3 direccion = new Vector3(
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(0.5f, 1f),
            UnityEngine.Random.Range(-1f, 1f)
        ).normalized;

        rb.AddForce(direccion * explosionForce, ForceMode.Impulse);
        rb.AddTorque(UnityEngine.Random.insideUnitSphere * explosionForce, ForceMode.Impulse);

        // Destruir después de 3 segundos
        Destroy(obj, 3f);
    }

    void DesaparecerCofre()
    {
        // Desactivar visualmente
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // Destruir completamente después de 4 segundos
        Destroy(gameObject, 4f);

        Debug.Log("🎁 Cofre recolectado y destruido");
    }
}