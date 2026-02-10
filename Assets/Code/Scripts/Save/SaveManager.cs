using System.Collections.Generic;
using System.IO;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if(Keyboard.current.f5Key.wasPressedThisFrame)
        {
            SaveGame();
        }
        if(Keyboard.current.f9Key.wasPressedThisFrame)
        {
            LoadGame();
        }
    }

   public void SaveGame()
{
    SaveData data = new();
    PanelShopElement[] shops = FindObjectsOfType<PanelShopElement>();

    Debug.Log($"🔍 Nombre de shops trouvés : {shops.Length}");

    foreach(var shop in shops)
    {
        if(!shop.HasBoughtZone())
            continue;

        Debug.Log($"🏪 Shop '{shop.GetShopName()}' - Éléments: {shop.allShopElement.Count}");

        ShopSaveData shopData = new();
        shopData.shopName = shop.GetShopName();
        shopData.shopType = (int) shop.GetSelectedShopType();

        foreach(var ground in shop.GetGroundElements())
        {
            shopData.boughtTiles.Add(ground.gridIndex);
        }

        // VOTRE CODE ORIGINAL QUI MARCHE
        foreach(var element in shop.allShopElement)
        {
            Debug.Log($"  📦 Traitement élément : {element?.name ?? "NULL"}");
            
            if(element == null)
            {
                Debug.LogWarning("  ⚠️ element est NULL, skip");
                continue;
            }
            
            if(element.wallElement == null)
            {
                Debug.LogWarning($"  ⚠️ wallElement est NULL sur {element.name}, skip");
                continue;
            }
            
            Debug.Log($"     ✓ wallElement OK: {element.wallElement.name}");

            // ===== CORRECTION ICI =====
            BlueprintWallElement wall = element.wallElement.GetComponent<BlueprintWallElement>();
            if(wall == null)
            {
                Debug.LogWarning($"  ⚠️ BlueprintWallElement null sur {element.wallElement.name}, skip");
                continue;
            }
            
            Debug.Log($"     ✓ BlueprintWallElement OK, wallKey: a={wall.wallKey.a}, b={wall.wallKey.b}");

            if(wall.wallInGame == null)
            {
                Debug.LogWarning($"  ⚠️ wallInGame null sur {wall.name}, skip");
                continue;
            }
            
            Debug.Log($"     ✓ wallInGame OK: {wall.wallInGame.name}");

            WallInMallElement worldElement = wall.wallInGame.GetComponent<WallInMallElement>();
            if(worldElement == null)
            {
                Debug.LogWarning($"  ⚠️ WallInMallElement manquant sur {wall.wallInGame.name}, skip");
                continue;
            }
            
            Debug.Log($"     ✓ WallInMallElement OK, type: {worldElement.downElement.elementType}");

            PlacedElementData elemData = new()
            {
                wallCellA = wall.wallKey.a,
                wallCellB = wall.wallKey.b,
                elementType = (int)worldElement.downElement.elementType,
            };

            shopData.placedElements.Add(elemData);
            Debug.Log($"  ✅ Élément sauvegardé : Type={worldElement.downElement.elementType}, A={wall.wallKey.a}, B={wall.wallKey.b}");
        }

        // SAUVER LES MURS MODIFIÉS (portes/fenêtres sans MapTooltipsElement)
        foreach(var wallKey in shop.allWallKeys)
        {
            if(!PanelShopMaster.instance.createdWalls.TryGetValue(wallKey, out GameObject wallObj))
                continue;

            BlueprintWallElement blueprintWall = wallObj.GetComponent<BlueprintWallElement>();
            if(blueprintWall == null || blueprintWall.wallInGame == null)
                continue;

            WallInMallElement wallInMall = blueprintWall.wallInGame.GetComponent<WallInMallElement>();
            if(wallInMall == null || wallInMall.downElement == null)
                continue;

            ElementType wallType = wallInMall.downElement.elementType;
            
            // Sauvegarder seulement si ce n'est PAS un mur normal ET qu'il n'a pas déjà été sauvegardé via allShopElement
            if(wallType != ElementType.Wall)
            {
                // Vérifier si cet élément n'est pas déjà dans la liste (éviter les doublons)
                bool alreadySaved = false;
                foreach(var existing in shopData.placedElements)
                {
                    if(existing.wallCellA == wallKey.a && existing.wallCellB == wallKey.b)
                    {
                        alreadySaved = true;
                        break;
                    }
                }
                
                if(!alreadySaved)
                {
                    PlacedElementData wallData = new()
                    {
                        wallCellA = wallKey.a,
                        wallCellB = wallKey.b,
                        elementType = (int)wallType
                    };
                    
                    shopData.placedElements.Add(wallData);
                    Debug.Log($"  🧱 Mur modifié sauvegardé : Type={wallType}, A={wallKey.a}, B={wallKey.b}");
                }
            }
        }

        Debug.Log($"💾 Éléments sauvegardés pour ce shop : {shopData.placedElements.Count}");
        data.shops.Add(shopData);
    }

    string json = JsonUtility.ToJson(data, true);
    File.WriteAllText(SavePath, json);
    Debug.Log($"✅ Sauvegarde terminée : {SavePath}");
}
    public void LoadGame()
    {
        if(!File.Exists(SavePath))
        {
            Debug.Log("Aucun fichier de sauvegarde");
            return;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        ClearCurrentGame();

        foreach(var shopData in data.shops)
        {
            LoadShop(shopData);
        }

    }

    void ClearCurrentGame()
    {
        PanelShopElement[] shops = FindObjectsOfType<PanelShopElement>();

        foreach(var shop in shops)
        {
            shop.ClearShop();
            Destroy(shop.gameObject);
        }

        foreach(var ground in PanelShopMaster.instance.grid.Values)
        {
            ground.CleanGround();
        }

        PanelShopMaster.instance.createdWalls.Clear();
    }

    void LoadShop(ShopSaveData data)
    {
        PanelShopElement panel = PanelShopMaster.instance.CreateEmptyShopPanel();
        panel.LoadFromSave(data);

        List<BlueprintGroundElement> tiles = new();

        // Charger les tuiles achetées
        foreach (var index in data.boughtTiles)
        {
            if (PanelShopMaster.instance.grid.TryGetValue(index, out var tile))
            {
                tiles.Add(tile);
            }
        }

        foreach (var tile in tiles)
        {
            tile.GroundBuy(data.shopName);
            panel.GetGroundElements().Add(tile);
        }

        // Créer les murs autour de la zone
        PanelShopMaster.instance.BuildWallsAroundZone(tiles, panel);
        panel.CreateShopVolume();

        
        Debug.Log($"✅ Shop '{data.shopName}' chargé avec succès");
    }
}
