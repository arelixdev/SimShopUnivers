using UnityEngine;
using UnityEngine.InputSystem;

public class PlacementWallElement : ShopPlaceableElement
{
     [SerializeField] private GameObject wallConstructor;
    private GameObject constructorInstance;

    private WallConstructorPivot currentPivotHover = null;

    private BlueprintGroundElement hoveredCell = null;
    private Direc hoveredDirection = Direc.North;

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

        hoveredCell = ground; // stocker la tuile survolée

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

    public void OnPivotHover(WallConstructorPivot pivot)
    {
        if (currentPivotHover != pivot)
        {
            currentPivotHover?.Hide();
            currentPivotHover = pivot;
            currentPivotHover.Show();

            // Stocker la direction du pivot
            hoveredDirection = pivot.pivotDirection;
        }

        uiInstance?.SetActive(false);
    }

    public void SelectPivot(WallConstructorPivot pivot)
    {
        if (!isPlacing) return;

        // Désélection ancien pivot
        if (currentPivotHover != null && currentPivotHover != pivot)
            currentPivotHover.Hide();

        currentPivotHover = pivot;
        currentPivotHover.Show();

        // Référence au parent (la bonne position)
        Transform t = pivot.transform.parent;

        Vector2Int dirVec = hoveredDirection switch
        {
            Direc.North => new Vector2Int(0, 1),
            Direc.South => new Vector2Int(0, -1),
            Direc.East  => new Vector2Int(1, 0),
            Direc.West  => new Vector2Int(-1, 0),
            _ => Vector2Int.zero
        };

        WallKey key = new WallKey(hoveredCell.gridIndex, dirVec);

        if (!PanelShopMaster.instance.createdWalls.ContainsKey(key))
        {
            Vector3 basePos = hoveredCell.WorldPosition(PanelShopMaster.instance.cellSize);
            Vector3 wallPos = basePos;
            Quaternion rotGame = Quaternion.identity;

            switch (hoveredDirection)
            {
                case Direc.East:  wallPos += new Vector3(PanelShopMaster.instance.cellSize, 0, PanelShopMaster.instance.cellSize); rotGame = Quaternion.Euler(0,0,0); break;
                case Direc.West:  wallPos += new Vector3(0,0,0); rotGame = Quaternion.Euler(0,180,0); break;
                case Direc.North: wallPos += new Vector3(PanelShopMaster.instance.cellSize,0,PanelShopMaster.instance.cellSize); rotGame = Quaternion.Euler(0,90,0); break;
                case Direc.South: wallPos += new Vector3(0,0,0); rotGame = Quaternion.Euler(0,-90,0); break;
            }

            rotGame *= Quaternion.Euler(0, -90, 0);

            GameObject wallGame = Instantiate(
                PanelShopMaster.instance.wallPrefabGame,
                wallPos,
                rotGame,
                PanelShopMaster.instance.wallsParentGame
            );

            PanelShopMaster.instance.GetPanelShopSelected().allWallGameShop.Add(wallGame);

            PanelShopMaster.instance.createdWalls.Add(key, wallGame);
        }

        

        // ----------- PREVIEW (planInstance existant) -----------
        if (planInstance != null)
        {
            planInstance.SetActive(true);
            planInstance.transform.position = t.position;
            planInstance.transform.rotation = t.rotation;
        }

        // ----------- CREATION DU MUR DEFINITIF -----------
        if (planPrefab != null)
        {
            GameObject placed = Instantiate(planPrefab, planParent);
            placed.transform.position = t.position;
            placed.transform.rotation = t.rotation * Quaternion.Euler(0f, 90f, 0f);

            PanelShopMaster.instance.GetPanelShopSelected().allWallShop.Add(placed);
        }

        planInstance?.SetActive(false);

        // UI off
        if (uiInstance != null)
            uiInstance.SetActive(false);

        // cacher le pivot
        pivot.Hide();
    }
}
