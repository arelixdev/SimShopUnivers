using UnityEngine;

public class WorkerController : MonoBehaviour
{
    public TypeWorkers typeWork;
}

public enum TypeWorkers
{
    Sellers,
    Storage,
    Janitor,
    Officier,
    Cooker
}
