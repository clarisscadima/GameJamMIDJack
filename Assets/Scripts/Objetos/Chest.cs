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
            RecolectarCofre();
    }

    void RecolectarCofre()
    {
        yaRecolectado = true;
        DarRecursosAlJugador();
        ExpulsarObjetosVisuales();
        DesaparecerCofre();
    }

    void DarRecursosAlJugador()
    {
        PlayerUI uiSystem = FindObjectOfType<PlayerUI>();
        InventorySystem inventorySystem = FindObjectOfType<InventorySystem>();

        if (uiSystem != null)
            uiSystem.CambiarVida(vidaParaDar);

        if (inventorySystem != null)
            inventorySystem.AgregarItem("Balas", balasParaDar);

        if (uiSystem != null)
            uiSystem.CambiarFichas(fichasParaDar);
    }

    void ExpulsarObjetosVisuales()
    {
        Vector3 posicionCofre = transform.position;

        for (int i = 0; i < 3; i++)
            if (heartEffect != null) CrearObjetoVolador(heartEffect, posicionCofre);

        for (int i = 0; i < 2; i++)
            if (bulletEffect != null) CrearObjetoVolador(bulletEffect, posicionCofre);

        for (int i = 0; i < 4; i++)
            if (coinEffect != null) CrearObjetoVolador(coinEffect, posicionCofre);
    }

    void CrearObjetoVolador(GameObject prefab, Vector3 posicion)
    {
        GameObject obj = Instantiate(prefab, posicion, UnityEngine.Random.rotation);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null) rb = obj.AddComponent<Rigidbody>();

        Vector3 direccion = new Vector3(
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(0.5f, 1f),
            UnityEngine.Random.Range(-1f, 1f)
        ).normalized;

        rb.AddForce(direccion * explosionForce, ForceMode.Impulse);
        rb.AddTorque(UnityEngine.Random.insideUnitSphere * explosionForce, ForceMode.Impulse);
        Destroy(obj, 3f);
    }

    void DesaparecerCofre()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 4f);
    }
}