using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GunPartInventoryLink
{
    public GunPart gunPart;
    public InventoryItemData itemData;
}

public class GunPartInventoryMap : MonoBehaviour
{
    [Header("Map GunPart -> InventoryItemData")]
    public List<GunPartInventoryLink> links = new();

    private Dictionary<GunPart, InventoryItemData> cache;

    private void Awake()
    {
        cache = new Dictionary<GunPart, InventoryItemData>();
        foreach (var l in links)
        {
            if (l == null || l.gunPart == null || l.itemData == null) continue;
            if (!cache.ContainsKey(l.gunPart))
                cache.Add(l.gunPart, l.itemData);
        }
    }

    public bool TryGetItemData(GunPart part, out InventoryItemData data)
    {
        data = null;
        if (part == null) return false;

        if (cache == null) Awake();
        return cache.TryGetValue(part, out data);
    }
}