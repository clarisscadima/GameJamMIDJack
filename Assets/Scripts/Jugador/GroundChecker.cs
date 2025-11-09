
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public Transform groundCheck;
    public float checkDistance = 1.1f;
    public LayerMask groundMask = 1; // Layer por defecto

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            CheckGround();
        }
    }

    void CheckGround()
    {
        Debug.Log("=== DEBUG SUELO ===");
        Debug.Log($"Player position: {transform.position}");

        if (groundCheck == null)
        {
            Debug.LogError("? GroundCheck no asignado!");
            return;
        }

        Debug.Log($"GroundCheck position: {groundCheck.position}");

        // Raycast hacia abajo
        RaycastHit hit;
        bool hasHit = Physics.Raycast(groundCheck.position, Vector3.down, out hit, checkDistance, groundMask);

        Debug.Log($"Raycast hit: {hasHit}");
        if (hasHit)
        {
            Debug.Log($"Chocó con: {hit.collider.gameObject.name}");
            Debug.Log($"Distancia: {hit.distance}");
        }

        // También verificar collider
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, 0.3f, groundMask);
        Debug.Log($"Colliders detectados: {colliders.Length}");
        foreach (Collider col in colliders)
        {
            Debug.Log($" - {col.gameObject.name}");
        }
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, 0.3f);
            Gizmos.DrawRay(groundCheck.position, Vector3.down * checkDistance);
        }
    }
}