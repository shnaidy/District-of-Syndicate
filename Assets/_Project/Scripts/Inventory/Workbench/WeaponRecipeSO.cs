using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponRecipe", menuName = "Blackmarket/Weapon Recipe")]
public class WeaponRecipeSO : ScriptableObject
{
    [System.Serializable]
    public class Ingredient
    {
        public InventoryItemData item;
        public int amount = 1;
    }

    [Header("Recipe Info")]
    public string recipeId = "ak47_recipe";
    public string displayName = "AK-47";
    [TextArea] public string description;

    [Header("Ingredients (required parts)")]
    public List<Ingredient> ingredients = new List<Ingredient>();

    [Header("Output (crafted weapon)")]
    public InventoryItemData outputItem;
    public int outputAmount = 1;
}