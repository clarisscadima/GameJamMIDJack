using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InputManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("InputManager");
                    instance = go.AddComponent<InputManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    private bool inputsBloqueados = false;
    private int canvasActivosCount = 0;
    private List<GameObject> canvasRegistrados = new List<GameObject>();

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static bool PuedeUsarInputs()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }

        if (Instance.inputsBloqueados || Instance.canvasActivosCount > 0)
        {
            return false;
        }

        if (Instance.canvasRegistrados.Count > 0)
        {
            Instance.canvasRegistrados.RemoveAll(c => c == null || !c.activeSelf);
            if (Instance.canvasRegistrados.Count > 0)
            {
                return false;
            }
        }

        return true;
    }

    public static bool MouseSobreUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    public static void BloquearInputs()
    {
        Instance.inputsBloqueados = true;
    }

    public static void DesbloquearInputs()
    {
        Instance.inputsBloqueados = false;
    }

    public static void RegistrarCanvasActivo()
    {
        Instance.canvasActivosCount++;
    }

    public static void DesregistrarCanvasActivo()
    {
        Instance.canvasActivosCount--;
        if (Instance.canvasActivosCount < 0) Instance.canvasActivosCount = 0;
    }

    public static void RegistrarCanvas(GameObject canvas)
    {
        if (canvas == null) return;
        if (!Instance.canvasRegistrados.Contains(canvas))
        {
            Instance.canvasRegistrados.Add(canvas);
        }
    }

    public static void DesregistrarCanvas(GameObject canvas)
    {
        if (canvas == null) return;
        if (Instance.canvasRegistrados.Contains(canvas))
        {
            Instance.canvasRegistrados.Remove(canvas);
        }
    }

    public static int GetCanvasActivos()
    {
        return Instance.canvasActivosCount;
    }

    public static int GetCanvasRegistrados()
    {
        Instance.canvasRegistrados.RemoveAll(c => c == null || !c.activeSelf);
        return Instance.canvasRegistrados.Count;
    }

    public static void Reset()
    {
        Instance.inputsBloqueados = false;
        Instance.canvasActivosCount = 0;
        Instance.canvasRegistrados.Clear();
    }
}