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

        foreach(var shop in shops)
        {
            if(!shop.HasBoughtZone())
                continue;

            ShopSaveData shopData = new();
            shopData.shopName = shop.GetShopName();
            shopData.shopType = (int) shop.GetSelectedShopType();

            foreach(var ground in shop.GetGroundElements())
            {
                shopData.boughtTiles.Add(ground.gridIndex);
            }

            data.shops.Add(shopData);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
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
        PanelShopMaster.instance.AddShop();
        PanelShopElement panel = PanelShopMaster.instance.GetPanelShopSelected();

        panel.GetComponentInChildren<TMPro.TMP_InputField>().text = data.shopName;
        panel.RenameShopElement();

        panel.ActualizeDd();
        panel.GetComponentInChildren<TMPro.TMP_Dropdown>().value = data.shopType;

        List<BlueprintGroundElement> tiles = new();

        foreach(var index in data.boughtTiles)
        {
            if(PanelShopMaster.instance.grid.TryGetValue(index, out var tile))
            {
                tiles.Add(tile);
            }
        }

        panel.GetGroundElements().Clear();
        panel.GetGroundElements().AddRange(tiles);

        //TODO not sure about this because we save "money" 
        foreach(var tile in tiles)
        {
            tile.GroundBuy(data.shopName);
        }

        PanelShopMaster.instance.BuildWallsAroundZone(tiles, panel);

        panel.RebuildShopWalls();
    }
}
