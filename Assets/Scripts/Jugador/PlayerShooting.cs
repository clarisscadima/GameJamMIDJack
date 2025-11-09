using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Referencias")]
    public Transform puntoDisparo;
    public GameObject balaPrefab;
    public KeyCode teclaDisparo = KeyCode.Mouse0;

    [Header("Configuración Disparo")]
    public float velocidadBala = 15f;
    public float cadenciaDisparo = 0.5f;

    private float ultimoDisparo;
    private InventorySystem inventario;

    void Start()
    {
        inventario = FindObjectOfType<InventorySystem>();
        if (puntoDisparo == null)
        {
            GameObject punto = new GameObject("PuntoDisparo");
            punto.transform.SetParent(transform);
            punto.transform.localPosition = new Vector3(0, 0.5f, 1.5f);
            puntoDisparo = punto.transform;
        }
    }

    void Update()
    {
        if (!InputManager.PuedeUsarInputs()) return;

        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(teclaDisparo)) &&
            Time.time >= ultimoDisparo + cadenciaDisparo)
        {
            IntentarDisparar();
        }
    }

    void IntentarDisparar()
    {
        if (!InputManager.PuedeUsarInputs()) return;
        if (inventario == null) return;
        if (!inventario.TieneItem("Revolver", 1)) return;
        if (!inventario.TieneItem("Balas", 1)) return;

        Disparar();
        inventario.UsarItem("Balas", 1);
    }

    void Disparar()
    {
        if (balaPrefab == null) return;
        if (puntoDisparo == null) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 direccionDisparo = cam.transform.forward;
        Vector3 spawnPosition = puntoDisparo.position + direccionDisparo * 0.5f;
        GameObject bala = Instantiate(balaPrefab, spawnPosition, Quaternion.LookRotation(direccionDisparo));

        BalaPlayer proyectil = bala.GetComponent<BalaPlayer>();
        if (proyectil != null)
        {
            proyectil.velocidad = velocidadBala;
            proyectil.direccion = direccionDisparo;
        }

        ultimoDisparo = Time.time;
    }

    void OnDrawGizmos()
    {
        if (puntoDisparo != null && Camera.main != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puntoDisparo.position, 0.1f);
            Gizmos.DrawRay(puntoDisparo.position, Camera.main.transform.forward * 2f);
        }
    }

    public bool PuedeDisparar()
    {
        return InputManager.PuedeUsarInputs() &&
               inventario != null &&
               inventario.TieneItem("Revolver", 1) &&
               inventario.TieneItem("Balas", 1);
    }
}