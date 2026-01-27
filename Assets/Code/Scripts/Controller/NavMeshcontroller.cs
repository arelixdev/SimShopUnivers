using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;

public class NavMeshcontroller : MonoBehaviour
{
    public static NavMeshcontroller instance;

    [SerializeField] private List<GameObject> obstacleCloseDoor;

    private void Awake() {
        instance = this;
    }

    public void RebuildNavMesh()
    {
    }

    public void OpenShopUpdate()
    {
        foreach(var obj in obstacleCloseDoor)
        {
            obj.SetActive(false);
        }
    }

    public void CloseShopUpdate()
    {
        foreach(var obj in obstacleCloseDoor)
        {
            obj.SetActive(true);
        }
    }
}
