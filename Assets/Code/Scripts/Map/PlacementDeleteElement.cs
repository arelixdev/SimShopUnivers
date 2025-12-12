using UnityEngine;
using UnityEngine.InputSystem;

public class PlacementDeleteElement : ShopPlaceableElement
{
    public override void StartPlacing()
    {
        base.StartPlacing();

        ignoreWallSnap = true;
    }
    protected override void FollowMouse()
    {
        if (Mouse.current == null)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (uiInstance != null)
            uiInstance.transform.position = mousePos;
    }
}
