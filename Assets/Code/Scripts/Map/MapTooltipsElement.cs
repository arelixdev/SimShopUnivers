using UnityEngine;

public class MapTooltipsElement : MonoBehaviour
{
    public ElementType elementType;

    public int valueElement = -1;
    public Sprite elementSprite;

    public Transform wallElement;

    public void ClearElement()
    {
        wallElement.gameObject.GetComponent<BlueprintWallElement>().wallInGame.GetComponent<WallInMallElement>().HideOtherElement();
        wallElement.gameObject.GetComponent<BlueprintWallElement>().ShowWall();

        Destroy(gameObject);
    }
}
