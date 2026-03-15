using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public InventoryManager inventory;

    [Header("AK-47 Recipe")]
    public string barrelPart = "Israel Barrel A";
    public string bodyPart = "Poland Body A";
    public string stockPart = "Poland Stock B";
    public string magPart = "Russia Mag A";

    public string craftedWeaponName = "AK-47";

    void Start()
    {
        if (inventory == null)
            inventory = FindObjectOfType<InventoryManager>();

        Debug.Log("Crafting: stiskni K pro pokus složit AK-47");
        Debug.Log("DEBUG: stiskni L pro přidání dílů receptu");
    }

    void Update()
    {
        // Craft
        if (Input.GetKeyDown(KeyCode.K))
        {
            TryCraftAK47();
        }

        // DEBUG: doplní 1x všechny potřebné díly pro recept
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddDebugRecipeParts();
        }
    }

    public void TryCraftAK47()
    {
        if (inventory == null)
        {
            Debug.LogError("CraftingManager nemá InventoryManager referenci.");
            return;
        }

        bool hasAllParts =
            inventory.HasItem(barrelPart, 1) &&
            inventory.HasItem(bodyPart, 1) &&
            inventory.HasItem(stockPart, 1) &&
            inventory.HasItem(magPart, 1);

        if (!hasAllParts)
        {
            Debug.LogWarning("Nelze craftit AK-47: chybí potřebné díly.");
            Debug.Log($"Potřebuješ: {barrelPart}, {bodyPart}, {stockPart}, {magPart}");
            PrintMissingParts();
            return;
        }

        inventory.RemoveItem(barrelPart, 1);
        inventory.RemoveItem(bodyPart, 1);
        inventory.RemoveItem(stockPart, 1);
        inventory.RemoveItem(magPart, 1);

        inventory.AddItem(craftedWeaponName, 1);

        Debug.Log("ÚSPĚCH: Složil jsi AK-47!");
        inventory.PrintInventory();
    }

    void AddDebugRecipeParts()
    {
        if (inventory == null)
        {
            Debug.LogError("InventoryManager reference je null.");
            return;
        }

        inventory.AddItem(barrelPart, 1);
        inventory.AddItem(bodyPart, 1);
        inventory.AddItem(stockPart, 1);
        inventory.AddItem(magPart, 1);

        Debug.Log("DEBUG: Přidány díly pro AK-47 recept.");
        inventory.PrintInventory();
    }

    void PrintMissingParts()
    {
        if (!inventory.HasItem(barrelPart, 1))
            Debug.LogWarning($"Chybí: {barrelPart}");

        if (!inventory.HasItem(bodyPart, 1))
            Debug.LogWarning($"Chybí: {bodyPart}");

        if (!inventory.HasItem(stockPart, 1))
            Debug.LogWarning($"Chybí: {stockPart}");

        if (!inventory.HasItem(magPart, 1))
            Debug.LogWarning($"Chybí: {magPart}");
    }
}