using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public List<ShopSaveData> shops = new();
}

[Serializable]
public class ShopSaveData
{
    public string shopName;
    public int shopType;

    public bool isBuy;
    public List<Vector2Int> boughtTiles = new();
    public List<PlacedElementData> placedElements = new(); 

    public List<FurnitureSaveData> shelvings = new();
    public List<FurnitureSaveData> checkouts = new();
}

[Serializable]
public class PlacedElementData
{
    public Vector2Int wallCellA;      // Premier point de la WallKey
    public Vector2Int wallCellB;      // Second point de la WallKey
    public int elementType;           // 0=Wall, 1=Door, 2=Window, 3=ShopWindow
}

[Serializable]
public class FurnitureSaveData
{
    public Vector3 position;      // Position dans le monde
    public Quaternion rotation;
    public int furnitureType;     // 0 = Shelf, 1 = Checkout
}
