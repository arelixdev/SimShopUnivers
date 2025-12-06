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
}


