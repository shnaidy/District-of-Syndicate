using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventoryV2 : MonoBehaviour
{
    [Header("Runtime Items")]
    [SerializeField] private List<InventoryItemInstance> items = new();

    public IReadOnlyList<InventoryItemInstance> Items => items;

    public event Action OnInventoryChanged;

    // ---------- ADD ----------
    public InventoryItemInstance AddItem(
        InventoryItemData data,
        int amount = 1,
        float durability01 = 1f,
        string countryOfOrigin = "",
        string source = "Unknown",
        int boughtPrice = 0,
        string serialNumber = "")
    {
        if (data == null)
        {
            Debug.LogWarning("[InventoryV2] AddItem failed: data is null.");
            return null;
        }

        if (amount <= 0) amount = 1;
        durability01 = Mathf.Clamp01(durability01);

        var incoming = CreateInstance(data, amount, durability01, countryOfOrigin, source, boughtPrice, serialNumber);

        if (data.stackable)
        {
            // try stack into existing compatible stacks
            int remaining = incoming.amount;

            foreach (var existing in items)
            {
                if (!existing.CanStackWith(incoming)) continue;

                int free = Mathf.Max(0, data.maxStack - existing.amount);
                if (free <= 0) continue;

                int toMove = Mathf.Min(free, remaining);
                existing.amount += toMove;
                remaining -= toMove;
                if (remaining <= 0)
                {
                    RaiseChanged();
                    return existing;
                }
            }

            // create new stacks for remaining amount
            while (remaining > 0)
            {
                int chunk = Mathf.Min(remaining, data.maxStack);
                var stack = CreateInstance(data, chunk, durability01, countryOfOrigin, source, boughtPrice, serialNumber);
                items.Add(stack);
                remaining -= chunk;
            }

            RaiseChanged();
            return incoming;
        }
        else
        {
            // non-stackable => one instance per piece
            for (int i = 0; i < amount; i++)
            {
                var one = CreateInstance(data, 1, durability01, countryOfOrigin, source, boughtPrice, serialNumber);
                items.Add(one);
            }

            RaiseChanged();
            return incoming;
        }
    }

    // ---------- REMOVE ----------
    public bool RemoveByInstanceId(string instanceId, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(instanceId)) return false;

        int index = items.FindIndex(x => x.instanceId == instanceId);
        if (index < 0) return false;

        var item = items[index];

        if (!item.data.stackable)
        {
            items.RemoveAt(index);
            RaiseChanged();
            return true;
        }

        if (amount <= 0) amount = 1;
        if (item.amount < amount) return false;

        item.amount -= amount;
        if (item.amount <= 0) items.RemoveAt(index);

        RaiseChanged();
        return true;
    }

    public bool RemoveByItemId(string itemId, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(itemId) || amount <= 0) return false;

        int remaining = amount;

        // remove from stacks first (largest stacks first can reduce fragmentation)
        var matches = items
            .Where(i => i.data != null && i.data.itemId == itemId)
            .OrderByDescending(i => i.amount)
            .ToList();

        if (matches.Sum(m => m.amount) < amount) return false;

        foreach (var m in matches)
        {
            if (remaining <= 0) break;

            int take = Mathf.Min(m.amount, remaining);
            m.amount -= take;
            remaining -= take;
        }

        items.RemoveAll(i => i.amount <= 0 || i.data == null);
        RaiseChanged();
        return true;
    }

    // ---------- QUERY ----------
    public int CountByItemId(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId)) return 0;
        return items.Where(i => i.data != null && i.data.itemId == itemId).Sum(i => i.amount);
    }

    public bool HasItem(string itemId, int amount = 1) => CountByItemId(itemId) >= amount;

    public InventoryItemInstance FindFirstByItemId(string itemId)
    {
        return items.FirstOrDefault(i => i.data != null && i.data.itemId == itemId);
    }

    public List<InventoryItemInstance> FindAllByCategory(InventoryItemCategory category)
    {
        return items.Where(i => i.data != null && i.data.category == category).ToList();
    }

    public int GetTotalItemCount() => items.Sum(i => Mathf.Max(0, i.amount));

    public void ClearAll()
    {
        items.Clear();
        RaiseChanged();
    }

    // ---------- INTERNAL ----------
    private InventoryItemInstance CreateInstance(
        InventoryItemData data,
        int amount,
        float durability01,
        string countryOfOrigin,
        string source,
        int boughtPrice,
        string serialNumber)
    {
        return new InventoryItemInstance
        {
            data = data,
            amount = Mathf.Max(1, amount),
            instanceId = Guid.NewGuid().ToString("N"),
            durability01 = Mathf.Clamp01(durability01),
            serialNumber = string.IsNullOrWhiteSpace(serialNumber) ? GenerateSerial() : serialNumber,
            countryOfOrigin = countryOfOrigin ?? "",
            source = source ?? "Unknown",
            boughtPrice = Mathf.Max(0, boughtPrice),
            acquiredUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
    }

    private static string GenerateSerial()
    {
        // short readable serial
        return $"SN-{UnityEngine.Random.Range(100000, 999999)}";
    }

    private void RaiseChanged()
    {
        OnInventoryChanged?.Invoke();
    }
}