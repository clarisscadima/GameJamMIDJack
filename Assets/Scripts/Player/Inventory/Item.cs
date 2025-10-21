using UnityEngine;

public class Item : MonoBehaviour
{
    public string nombreItem;
    public int cantidad = 1;

    void Start()
    {
        // Asegurarse de que tenga tag Item
        gameObject.tag = "Item";
    }
}