using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private InventorySystem inventario;

    void Start()
    {
        inventario = FindObjectOfType<InventorySystem>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();
            if (item != null)
            {
                inventario.AgregarItem(item.nombreItem, item.cantidad);
                Destroy(other.gameObject);
            }
        }
    }
}