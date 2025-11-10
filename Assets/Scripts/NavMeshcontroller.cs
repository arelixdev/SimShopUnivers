using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;

public class NavMeshcontroller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.fKey.wasPressedThisFrame)
        {
            GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }
}
