using System;
using UnityEngine;

public class WorkerController : MonoBehaviour
{
    public TypeWorkers typeWork = TypeWorkers.Standby;

    internal void OpenUIMenu()
    {
        WorkerMenu.instance.OpenMenu();
    }
}

public enum TypeWorkers
{
    Sellers,
    Storage,
    Janitor,
    Officier,
    Cooker,
    Standby
}
