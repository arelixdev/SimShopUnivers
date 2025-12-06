using UnityEngine;

public class BlueprintWallElement : MonoBehaviour
{
    public enum WallDirection
    {
        North,
        East,
        South,
        West
    }

    public WallDirection direction = WallDirection.North;

    public GameObject groundLink;

    private void OnTriggerStay(Collider other) {
        BlueprintGroundElement groundElement = other.GetComponent<BlueprintGroundElement>();
        if(groundElement != null && groundElement.isBuy && groundLink == null)
        {
            groundLink = groundElement.gameObject;
        }
    }
}


