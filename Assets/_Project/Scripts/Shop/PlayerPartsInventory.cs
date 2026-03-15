using System.Collections.Generic;
using UnityEngine;

public class PlayerPartsInventory : MonoBehaviour
{
    [Header("Runtime inventory")]
    [SerializeField] private List<GunPart> ownedParts = new();

    public IReadOnlyList<GunPart> OwnedParts => ownedParts;

    public void AddPart(GunPart part)
    {
        if (part == null)
        {
            Debug.LogWarning("[Inventory] AddPart called with NULL part.");
            return;
        }

        ownedParts.Add(part);
        Debug.Log($"[Inventory] Added part: {part.partName}. Total parts: {ownedParts.Count}");
    }

    public bool ContainsPart(GunPart part)
    {
        return part != null && ownedParts.Contains(part);
    }

    public bool RemovePart(GunPart part)
    {
        if (part == null) return false;
        bool removed = ownedParts.Remove(part);
        if (removed)
            Debug.Log($"[Inventory] Removed part: {part.partName}. Total parts: {ownedParts.Count}");
        return removed;
    }

    public void ClearInventory()
    {
        ownedParts.Clear();
        Debug.Log("[Inventory] Cleared.");
    }
}