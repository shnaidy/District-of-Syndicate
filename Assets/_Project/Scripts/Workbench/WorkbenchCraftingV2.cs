using System.Text;
using UnityEngine;

public class WorkbenchCraftingV2 : MonoBehaviour
{
    [Header("Refs")]
    public PlayerInventoryV2 inventoryV2;

    [Header("Recipes")]
    public WeaponRecipeSO[] recipes;

    private void Start()
    {
        if (inventoryV2 == null) inventoryV2 = FindObjectOfType<PlayerInventoryV2>();
    }

    public bool CanCraft(WeaponRecipeSO recipe)
    {
        if (recipe == null || inventoryV2 == null) return false;

        foreach (var ing in recipe.ingredients)
        {
            if (ing == null || ing.item == null) return false;
            int need = Mathf.Max(1, ing.amount);
            int have = inventoryV2.GetTotalCountByData(ing.item);
            if (have < need) return false;
        }
        return true;
    }

    public bool TryCraft(WeaponRecipeSO recipe)
    {
        if (recipe == null)
        {
            Debug.LogWarning("[WorkbenchV2] Recipe is null.");
            return false;
        }

        if (inventoryV2 == null)
        {
            Debug.LogError("[WorkbenchV2] Missing PlayerInventoryV2.");
            return false;
        }

        if (recipe.outputItem == null)
        {
            Debug.LogError("[WorkbenchV2] Recipe outputItem is null.");
            return false;
        }

        if (!CanCraft(recipe))
        {
            Debug.LogWarning(GetMissingIngredientsText(recipe));
            return false;
        }

        foreach (var ing in recipe.ingredients)
        {
            int need = Mathf.Max(1, ing.amount);
            inventoryV2.RemoveItemsByData(ing.item, need);
        }

        inventoryV2.AddItem(
            data: recipe.outputItem,
            amount: Mathf.Max(1, recipe.outputAmount),
            durability01: 1f,
            countryOfOrigin: "",
            source: "WorkbenchCraft",
            boughtPrice: 0
        );

        Debug.Log($"[WorkbenchV2] Crafted: {recipe.displayName}");
        return true;
    }

    public string GetMissingIngredientsText(WeaponRecipeSO recipe)
    {
        if (recipe == null || inventoryV2 == null) return "Missing refs.";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Missing for {recipe.displayName}:");

        foreach (var ing in recipe.ingredients)
        {
            if (ing == null || ing.item == null) continue;
            int need = Mathf.Max(1, ing.amount);
            int have = inventoryV2.GetTotalCountByData(ing.item);

            if (have < need)
                sb.AppendLine($"- {ing.item.displayName}: {have}/{need}");
        }

        return sb.ToString();
    }
}