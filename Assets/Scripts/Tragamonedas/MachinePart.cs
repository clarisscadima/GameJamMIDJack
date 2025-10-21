using UnityEngine;

public class MachinePart : MonoBehaviour
{
    public string nombreParte;
    private SlotMachineManager slotMachine;
    private bool yaRecolectada = false;

    void Start()
    {
        slotMachine = FindObjectOfType<SlotMachineManager>();

        // Verificar que se encontró el SlotMachineManager
        if (slotMachine == null)
        {
            Debug.LogError("No se encontró SlotMachineManager en la escena!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Verificar que no esté ya recolectada y que sea el jugador
        if (other.CompareTag("Player") && !yaRecolectada)
        {
            Debug.Log($"Parte encontrada: {nombreParte}");

            if (slotMachine != null)
            {
                slotMachine.EncontrarParte();
                yaRecolectada = true;

                // Desaparecer la parte visualmente
                DesaparecerParte();
            }
            else
            {
                Debug.LogError("SlotMachineManager es null!");
            }
        }
    }

    void DesaparecerParte()
    {
        // Desactivar el renderer para que no se vea
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.enabled = false;

        // Desactivar el collider para que no se pueda recoger again
        Collider collider = GetComponent<Collider>();
        if (collider != null)
            collider.enabled = false;

        // Opcional: agregar efecto de partículas
        // GameObject efecto = Instantiate(efectoRecoleccion, transform.position, Quaternion.identity);
        // Destroy(efecto, 2f);

        Debug.Log($"Parte {nombreParte} recolectada y desaparecida");
    }
}