using UnityEngine;

public class WorkerMenu : MonoBehaviour
{
    public static WorkerMenu instance;

    [SerializeField] private GameObject workerMenu;

    public GameObject GetWorkerMenu()
    {
        return workerMenu;
    }

    private void Awake()
    {
        instance = this;

        CloseMenu();
    }

    public void OpenMenu()
    {
        workerMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseMenu()
    {
        workerMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
}
