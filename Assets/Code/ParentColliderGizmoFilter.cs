using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ParentColliderGizmoFilter
{
    static ParentColliderGizmoFilter()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (Selection.activeGameObject == null) return;

        GameObject selected = Selection.activeGameObject;

        // Parcours tous les colliders de la scène
        Collider[] allColliders = Object.FindObjectsOfType<Collider>();
        foreach (Collider col in allColliders)
        {
            // Si le collider n'est pas sur le parent sélectionné, on le cache
            if (!col.transform.IsChildOf(selected.transform) && col.gameObject != selected)
                continue;

            // Affiche uniquement le collider du parent sélectionné
            if (col.gameObject == selected)
            {
                Handles.color = Color.green;
                if (col is BoxCollider box)
                {
                    Handles.DrawWireCube(box.bounds.center, box.bounds.size);
                }
            }
        }

        // Bloque le dessin par défaut des autres colliders
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
    }
}