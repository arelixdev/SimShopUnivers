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
    public List<Vector2Int> boughtTiles = new();
}
