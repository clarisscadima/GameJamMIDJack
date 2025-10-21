using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [Header("UI - TextMesh Pro")]
    public TMP_Text inventarioText;

    [Header("Configuración Inventario")]
    public int espacioMaximo = 10;

    // Diccionario para almacenar items y sus cantidades
    private Dictionary<string, int> inventario = new Dictionary<string, int>();

    void Start()
    {
        ActualizarUI();
    }

    void Update()
    {
        // Ejemplo: Teclas para testing (quitar después)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AgregarItem("Botella", 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AgregarItem("Revolver", 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AgregarItem("Balas", 6);
        }
    }

    public void AgregarItem(string nombreItem, int cantidad)
    {
        if (inventario.ContainsKey(nombreItem))
        {
            inventario[nombreItem] += cantidad;
        }
        else
        {
            inventario[nombreItem] = cantidad;
        }

        ActualizarUI();
        Debug.Log($"Agregado: {cantidad} {nombreItem}");
    }

    public void UsarItem(string nombreItem, int cantidad = 1)
    {
        if (inventario.ContainsKey(nombreItem))
        {
            inventario[nombreItem] -= cantidad;

            if (inventario[nombreItem] <= 0)
            {
                inventario.Remove(nombreItem);
            }

            ActualizarUI();
            Debug.Log($"Usado: {cantidad} {nombreItem}");
        }
    }

    public bool TieneItem(string nombreItem, int cantidad = 1)
    {
        return inventario.ContainsKey(nombreItem) && inventario[nombreItem] >= cantidad;
    }

    public int CantidadDeItem(string nombreItem)
    {
        return inventario.ContainsKey(nombreItem) ? inventario[nombreItem] : 0;
    }

    void ActualizarUI()
    {
        if (inventarioText != null)
        {
            string textoUI = "INVENTARIO:\n";

            if (inventario.Count == 0)
            {
                textoUI += "Vacío";
            }
            else
            {
                foreach (var item in inventario)
                {
                    textoUI += $"{item.Key}: {item.Value}\n";
                }
            }

            inventarioText.text = textoUI;
        }
    }
}