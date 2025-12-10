using UnityEngine;
using UnityEngine.InputSystem;

public class PlacementWallElement : ShopPlaceableElement
{
     [SerializeField] private GameObject wallConstructor;
    private GameObject constructorInstance;

    public override void StartPlacing()
    {
        base.StartPlacing();

        ignoreWallSnap = true;

        if (wallConstructor != null)
        {
            constructorInstance = Instantiate(wallConstructor, planParent); 
            constructorInstance.SetActive(true); 
        }
    }

    public override void OnGroundHover(BlueprintGroundElement ground)
    {
        if (!isPlacing) return;

        if (constructorInstance != null)
        {
            constructorInstance.SetActive(true);
            constructorInstance.transform.position = ground.transform.position;
        }
    }

    protected override void FollowMouse()
    {
        if (Mouse.current == null)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        // UI suit la souris
        if (uiInstance != null)
            uiInstance.transform.position = mousePos;
    }
}
