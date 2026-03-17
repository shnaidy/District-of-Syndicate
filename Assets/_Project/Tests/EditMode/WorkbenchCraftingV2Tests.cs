using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class WorkbenchCraftingV2Tests
{
    private readonly List<Object> createdObjects = new List<Object>();

    [TearDown]
    public void TearDown()
    {
        foreach (var obj in createdObjects)
        {
            if (obj != null)
            {
                Object.DestroyImmediate(obj);
            }
        }

        createdObjects.Clear();
    }

    [Test]
    public void TryCraft_WhenRecipeIsNull_ReturnsFalse()
    {
        var workbench = CreateComponent<WorkbenchCraftingV2>();

        bool crafted = workbench.TryCraft(null);

        Assert.IsFalse(crafted);
    }

    [Test]
    public void TryCraft_WhenInventoryIsMissing_ReturnsFalse()
    {
        var workbench = CreateComponent<WorkbenchCraftingV2>();
        var recipe = CreateRecipe();
        recipe.outputItem = CreateItemData("weapon.ak47", "AK-47");

        bool crafted = workbench.TryCraft(recipe);

        Assert.IsFalse(crafted);
    }

    [Test]
    public void TryCraft_WhenOutputItemIsMissing_ReturnsFalse()
    {
        var workbench = CreateComponent<WorkbenchCraftingV2>();
        workbench.inventoryV2 = CreateComponent<PlayerInventoryV2>();
        var recipe = CreateRecipe();

        bool crafted = workbench.TryCraft(recipe);

        Assert.IsFalse(crafted);
    }

    [Test]
    public void CanCraft_WhenIngredientsAreMissing_ReturnsFalse()
    {
        var inventory = CreateComponent<PlayerInventoryV2>();
        var workbench = CreateComponent<WorkbenchCraftingV2>();
        workbench.inventoryV2 = inventory;

        var barrel = CreateItemData("part.barrel", "Barrel");
        var recipe = CreateRecipe();
        recipe.ingredients.Add(new WeaponRecipeSO.Ingredient { item = barrel, amount = 2 });

        inventory.AddItem(barrel, amount: 1);

        bool canCraft = workbench.CanCraft(recipe);

        Assert.IsFalse(canCraft);
    }

    [Test]
    public void TryCraft_WhenRequirementsAreMet_ConsumesIngredientsAndAddsOutput()
    {
        var inventory = CreateComponent<PlayerInventoryV2>();
        var workbench = CreateComponent<WorkbenchCraftingV2>();
        workbench.inventoryV2 = inventory;

        var barrel = CreateItemData("part.barrel", "Barrel");
        barrel.stackable = true;
        barrel.maxStack = 99;

        var output = CreateItemData("weapon.ak47", "AK-47");
        output.stackable = false;

        var recipe = CreateRecipe();
        recipe.ingredients.Add(new WeaponRecipeSO.Ingredient { item = barrel, amount = 2 });
        recipe.outputItem = output;
        recipe.outputAmount = 1;

        inventory.AddItem(barrel, amount: 2);

        bool crafted = workbench.TryCraft(recipe);

        Assert.IsTrue(crafted);
        Assert.AreEqual(0, inventory.GetTotalCountByData(barrel));
        Assert.AreEqual(1, inventory.GetTotalCountByData(output));
    }

    private T CreateComponent<T>() where T : Component
    {
        var go = new GameObject(typeof(T).Name + "_Test");
        createdObjects.Add(go);
        return go.AddComponent<T>();
    }

    private InventoryItemData CreateItemData(string id, string displayName)
    {
        var data = ScriptableObject.CreateInstance<InventoryItemData>();
        data.itemId = id;
        data.displayName = displayName;
        createdObjects.Add(data);
        return data;
    }

    private WeaponRecipeSO CreateRecipe()
    {
        var recipe = ScriptableObject.CreateInstance<WeaponRecipeSO>();
        recipe.displayName = "Recipe";
        recipe.ingredients = new List<WeaponRecipeSO.Ingredient>();
        createdObjects.Add(recipe);
        return recipe;
    }
}
