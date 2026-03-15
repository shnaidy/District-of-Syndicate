using System.Text;
using UnityEngine;

public class InventoryDebugPrinter : MonoBehaviour
{
    public PlayerInventoryV2 inventory;

    [ContextMenu("Print Inventory")]
    public void PrintInventory()
    {
        if (inventory == null)
        {
            Debug.LogWarning("[InventoryDebug] inventory ref missing.");
            return;
        }

        var items = inventory.Items;
        if (items.Count == 0)
        {
            Debug.Log("=== INVENTORY V2 ===\n(empty)");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== INVENTORY V2 ===");
        foreach (var it in items)
        {
            if (it?.data == null) continue;
            sb.AppendLine(
                $"{it.data.displayName} x{it.amount} | id:{it.data.itemId} | inst:{it.instanceId[..8]} | dur:{it.durability01:0.00} | src:{it.source} | buy:${it.boughtPrice}");
        }

        Debug.Log(sb.ToString());
    }
}