using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;

public class NavMeshcontroller : MonoBehaviour
{
    public static NavMeshcontroller instance;

    private void Awake() {
        instance = this;
    }

    public void RebuildNavMesh()
    {
        //GetComponent<NavMeshSurface>().UpdateNavMesh(GetComponent<NavMeshSurface>().navMeshData);
    }

    public void OpenShopUpdate()
    {
        //GetComponent<NavMeshSurface>().useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
        //RebuildNavMesh();
    }

    public void CloseShopUpdate()
    {
        //GetComponent<NavMeshSurface>().useGeometry = UnityEngine.AI.NavMeshCollectGeometry.RenderMeshes;
        //RebuildNavMesh();
    }
}
