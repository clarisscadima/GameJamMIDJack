using UnityEngine;

public class MachinePart : MonoBehaviour
{
    public string nombreParte;
    private SlotMachineManager slotMachine;
    private bool yaRecolectada = false;

    void Start()
    {
        slotMachine = FindObjectOfType<SlotMachineManager>();
        if (slotMachine == null)
        {
            Debug.LogError("No se encontró SlotMachineManager en la escena!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !yaRecolectada)
        {
            Debug.Log($"Parte encontrada: {nombreParte}");
            if (slotMachine != null)
            {
                slotMachine.EncontrarParte();
                yaRecolectada = true;
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
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.enabled = false;

        Collider collider = GetComponent<Collider>();
        if (collider != null)
            collider.enabled = false;

        Debug.Log($"Parte {nombreParte} recolectada y desaparecida");
    }
}