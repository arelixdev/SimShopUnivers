using UnityEngine;

public class WallInMallElement : MonoBehaviour
{
    [SerializeField] private GameObject wallDown;
    [SerializeField] private GameObject wallDoor;

    void Start()
    {
        wallDoor.SetActive(false);
    }

    public void ShowDoor()
    {
        wallDown.SetActive(false);

        wallDoor.SetActive(true);
    }
}
