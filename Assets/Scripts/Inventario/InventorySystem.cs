using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemData
{
    public string nombre;
    public Sprite icono;
    public string descripcion;
    public int cantidadMaxStack = 99;
}

public class InventorySystem : MonoBehaviour
{
    [Header("UI - Texto Simple")]
    public TMP_Text inventarioText;

    [Header("UI - Panel Visual de Inventario")]
    public GameObject panelInventarioVisual;
    public GameObject slotItemPrefab;
    public Transform contenedorSlots;

    [Header("Configuración Inventario")]
    public int espacioMaximo = 10;

    [Header("Base de Datos de Items")]
    public List<ItemData> baseDatosItems = new List<ItemData>();

    private Dictionary<string, int> inventario = new Dictionary<string, int>();
    private List<GameObject> slotsVisuales = new List<GameObject>();
    private PlayerUI playerUI;
    private bool inventarioAbierto = false;

    void Start()
    {
        playerUI = FindObjectOfType<PlayerUI>();
        ActualizarUI();
        CrearSlotsVisuales();
        if (panelInventarioVisual != null)
            panelInventarioVisual.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
            ToggleInventarioVisual();
    }

    void CrearSlotsVisuales()
    {
        if (contenedorSlots == null || slotItemPrefab == null) return;

        foreach (var slot in slotsVisuales)
            if (slot != null) Destroy(slot);

        slotsVisuales.Clear();
        for (int i = 0; i < espacioMaximo; i++)
        {
            GameObject nuevoSlot = Instantiate(slotItemPrefab, contenedorSlots);
            slotsVisuales.Add(nuevoSlot);
            nuevoSlot.SetActive(false);
        }
    }

    void ToggleInventarioVisual()
    {
        if (panelInventarioVisual != null)
        {
            inventarioAbierto = !panelInventarioVisual.activeSelf;
            panelInventarioVisual.SetActive(inventarioAbierto);

            if (playerUI != null)
            {
                if (inventarioAbierto)
                    playerUI.ActivarModoUI();
                else
                    playerUI.DesactivarModoUI();
            }

            if (inventarioAbierto)
                ActualizarInventarioVisual();
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
            if (inventario.Count >= espacioMaximo)
            {
                MostrarMensaje("¡Inventario lleno!");
                return;
            }
            inventario[nombreItem] = cantidad;
        }
        ActualizarUI();
        ActualizarInventarioVisual();
        MostrarMensaje($"+{cantidad} {nombreItem}");
    }

    public void UsarItem(string nombreItem, int cantidad = 1)
    {
        if (inventario.ContainsKey(nombreItem) && inventario[nombreItem] >= cantidad)
        {
            inventario[nombreItem] -= cantidad;
            if (inventario[nombreItem] <= 0)
                inventario.Remove(nombreItem);
            ActualizarUI();
            ActualizarInventarioVisual();
            MostrarMensaje($"Usado: {cantidad} {nombreItem}");
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
                textoUI += "Vacío";
            else
                foreach (var item in inventario)
                    textoUI += $"{item.Key}: {item.Value}\n";
            inventarioText.text = textoUI;
        }
    }

    void ActualizarInventarioVisual()
    {
        if (slotsVisuales.Count == 0) return;

        int indice = 0;
        foreach (var item in inventario)
        {
            if (indice >= slotsVisuales.Count) break;
            GameObject slot = slotsVisuales[indice];
            slot.SetActive(true);

            UnityEngine.UI.Image imagenSlot = slot.transform.Find("IconoItem")?.GetComponent<UnityEngine.UI.Image>();
            TMP_Text textoSlot = slot.transform.Find("CantidadText")?.GetComponent<TMP_Text>();
            TMP_Text nombreSlot = slot.transform.Find("NombreText")?.GetComponent<TMP_Text>();

            ItemData datos = ObtenerDatosItem(item.Key);
            if (imagenSlot != null)
            {
                if (datos != null && datos.icono != null)
                {
                    imagenSlot.sprite = datos.icono;
                    imagenSlot.enabled = true;
                    imagenSlot.color = Color.white;
                }
                else
                {
                    imagenSlot.enabled = false;
                }
            }
            if (textoSlot != null)
                textoSlot.text = item.Value > 1 ? item.Value.ToString() : "";
            if (nombreSlot != null)
                nombreSlot.text = item.Key;

            indice++;
        }

        for (int i = indice; i < slotsVisuales.Count; i++)
            slotsVisuales[i].SetActive(false);
    }

    ItemData ObtenerDatosItem(string nombreItem)
    {
        foreach (var itemData in baseDatosItems)
            if (itemData.nombre == nombreItem) return itemData;
        return null;
    }

    void MostrarMensaje(string mensaje)
    {
        if (playerUI != null)
            playerUI.MostrarMensaje(mensaje);
    }

    public void LimpiarInventario()
    {
        inventario.Clear();
        ActualizarUI();
        ActualizarInventarioVisual();
        MostrarMensaje("Inventario limpiado");
    }

    public bool IsInventoryOpen()
    {
        return inventarioAbierto;
    }
}