using System.Text;
using UnityEngine;

public class InventoryV2KeyDebug : MonoBehaviour
{
    public PlayerInventoryV2 inventory;
    public KeyCode printKey = KeyCode.I;

    void Update()
    {
        if (Input.GetKeyDown(printKey))
            PrintInventory();
    }

    public void PrintInventory()
    {
        if (inventory == null)
        {
            Debug.LogWarning("[InventoryV2] Missing inventory reference.");
            return;
        }

        var items = inventory.Items;
        if (items.Count == 0)
        {
            Debug.Log("=== INVENTORY V2 ===\n(prázdný)");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== INVENTORY V2 ===");

        for (int i = 0; i < items.Count; i++)
        {
            var it = items[i];
            if (it == null || it.data == null) continue;

            sb.AppendLine(
                $"{i + 1}) {it.data.displayName} x{it.amount} | id:{it.data.itemId} | src:{it.source} | price:${it.boughtPrice} | country:{it.countryOfOrigin} | dur:{it.durability01:0.00}");
        }

        Debug.Log(sb.ToString());
    }
}