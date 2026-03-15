using UnityEngine;

public class SellingManager : MonoBehaviour
{
    [Header("References")]
    public InventoryManager inventory;
    public GameTimeManager gameTime;
    public HeatManager heatManager;
    public PlayerWallet wallet;

    [Header("Sell Settings")]
    public string weaponToSell = "AK-47";
    public int daySellPrice = 4500;
    public int nightSellPrice = 7000;

    [Header("Heat Impact")]
    public float dayHeatChange = -3f;   // přes den se může lehce čistit stopa
    public float nightHeatGain = 12f;   // v noci velký risk

    void Start()
    {
        if (inventory == null) inventory = FindObjectOfType<InventoryManager>();
        if (gameTime == null) gameTime = FindObjectOfType<GameTimeManager>();
        if (heatManager == null) heatManager = FindObjectOfType<HeatManager>();
        if (wallet == null) wallet = FindObjectOfType<PlayerWallet>();

        Debug.Log("Selling: stiskni P pro prodej AK-47");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TrySellWeapon();
        }
    }

    void TrySellWeapon()
    {
        if (inventory == null || gameTime == null || wallet == null)
        {
            Debug.LogError("SellingManager: chybí reference.");
            return;
        }

        if (!inventory.HasItem(weaponToSell, 1))
        {
            Debug.LogWarning($"Nemáš {weaponToSell} k prodeji.");
            return;
        }

        bool isNight = gameTime.IsNight();
        int sellPrice = isNight ? nightSellPrice : daySellPrice;

        inventory.RemoveItem(weaponToSell, 1);
        wallet.AddCash(sellPrice);

        if (heatManager != null)
        {
            if (isNight) heatManager.AddHeat(nightHeatGain);
            else heatManager.ReduceHeat(Mathf.Abs(dayHeatChange));
        }

        Debug.Log(isNight
            ? $"NOČNÍ DEAL: Prodal jsi {weaponToSell} za ${sellPrice}. (vyšší risk)"
            : $"DENNÍ PRODEJ: Prodal jsi {weaponToSell} za ${sellPrice}. (nižší risk)");

        Debug.Log($"[Cash] Nový zůstatek: ${wallet.cash}");

        if (heatManager != null)
            Debug.Log($"[HEAT] Aktuálně: {heatManager.currentHeat:0.0}/{heatManager.maxHeat}");
    }
}