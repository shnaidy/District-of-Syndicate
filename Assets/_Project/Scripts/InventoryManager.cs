using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private Dictionary<string, int> items = new Dictionary<string, int>();

    public void AddItem(string itemName, int amount = 1)
    {
        if (items.ContainsKey(itemName))
            items[itemName] += amount;
        else
            items[itemName] = amount;

        Debug.Log($"[Inventory] +{amount} {itemName} (celkem: {items[itemName]})");
    }

    public bool HasItem(string itemName, int amount = 1)
    {
        return items.ContainsKey(itemName) && items[itemName] >= amount;
    }

    public bool RemoveItem(string itemName, int amount = 1)
    {
        if (!HasItem(itemName, amount))
            return false;

        items[itemName] -= amount;
        if (items[itemName] <= 0)
            items.Remove(itemName);

        Debug.Log($"[Inventory] -{amount} {itemName}");
        return true;
    }

    public void PrintInventory()
    {
        Debug.Log("=== INVENTÁŘ ===");

        if (items.Count == 0)
        {
            Debug.Log("(prázdný)");
            return;
        }

        foreach (var kvp in items)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}x");
        }
    }
}