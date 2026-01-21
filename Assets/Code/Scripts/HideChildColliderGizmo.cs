using UnityEngine;

[ExecuteAlways]
public class HideChildCollidersGizmo : MonoBehaviour
{
    void OnDrawGizmos()
    {
        // Affiche uniquement le collider du parent
        Collider parentCollider = GetComponent<Collider>();
        if (parentCollider != null)
        {
            Gizmos.color = Color.green;
            if (parentCollider is BoxCollider box)
            {
                Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
            }
            // Ajouter d'autres types de collider si nécessaire
        }
    }
}