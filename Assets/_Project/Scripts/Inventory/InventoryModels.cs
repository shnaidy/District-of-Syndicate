using System;
using UnityEngine;

public enum InventoryItemCategory
{
    Part,
    Weapon,
    Material,
    Misc
}

[CreateAssetMenu(fileName = "InventoryItemData", menuName = "Inventory/Item Data")]
public class InventoryItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemId;              // must be unique, e.g. "part.ar15.barrel.gov16"
    public string displayName;
    public InventoryItemCategory category;

    [Header("Economy")]
    public int basePrice = 100;

    [Header("Stacking")]
    public bool stackable = false;
    public int maxStack = 99;

    [Header("Visual")]
    public Sprite icon;
}

[Serializable]
public class InventoryItemInstance
{
    [Header("Reference")]
    public InventoryItemData data;

    [Header("Amounts")]
    public int amount = 1; // for stackable items

    [Header("Instance Metadata")]
    public string instanceId;          // unique GUID
    public float durability01 = 1f;    // 0..1
    public string serialNumber;
    public string countryOfOrigin;
    public string source;              // e.g. "Shop", "BlackMarket", "Loot"
    public int boughtPrice;
    public long acquiredUnixTime;

    public bool IsValid => data != null && !string.IsNullOrWhiteSpace(instanceId);

    public bool CanStackWith(InventoryItemInstance other)
    {
        if (other == null || data == null || other.data == null) return false;
        if (!data.stackable || !other.data.stackable) return false;

        // stack only if same item + same relevant metadata
        return data.itemId == other.data.itemId
               && Mathf.Approximately(durability01, other.durability01)
               && countryOfOrigin == other.countryOfOrigin
               && boughtPrice == other.boughtPrice;
    }
}