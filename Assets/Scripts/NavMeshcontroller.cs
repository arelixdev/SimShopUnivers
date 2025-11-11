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
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
}
