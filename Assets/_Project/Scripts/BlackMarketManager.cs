using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BlackMarketManager : MonoBehaviour
{
    [System.Serializable]
    public class MarketOffer
    {
        public InventoryItemData itemData; // item, který se přidá do PlayerInventoryV2
        public string partName;            // jen display
        public int price;
        public float scamRisk; // 0.0 - 1.0
    }

    [Header("Reference")]
    public GameTimeManager gameTime;
    public PlayerInventoryV2 inventoryV2;

    [Header("Market Pool (InventoryItemData assets)")]
    public List<InventoryItemData> possibleItems = new List<InventoryItemData>();

    [Header("Market Settings")]
    public int refreshEveryDays = 3;
    public int offersPerRefresh = 4;

    [Header("Player Economy")]
    public int playerCash = 10000;

    private int lastRefreshDay = 1;
    private readonly List<MarketOffer> currentOffers = new List<MarketOffer>();

    void Start()
    {
        if (gameTime == null) gameTime = FindObjectOfType<GameTimeManager>();
        if (inventoryV2 == null) inventoryV2 = FindObjectOfType<PlayerInventoryV2>();

        GenerateOffers();
        lastRefreshDay = gameTime != null ? gameTime.day : 1;

        Debug.Log($"[Cash] Start cash: ${playerCash}");
        Debug.Log("Nákup blackmarketu probíhá pouze přes UI terminál (počítač).");
    }

    void Update()
    {
        if (gameTime == null) return;

        // Refresh market každé X dní
        int daysPassed = gameTime.day - lastRefreshDay;
        if (daysPassed >= refreshEveryDays)
        {
            GenerateOffers();
            lastRefreshDay = gameTime.day;
        }
    }

    void GenerateOffers()
    {
        currentOffers.Clear();

        if (possibleItems == null || possibleItems.Count == 0)
        {
            Debug.LogWarning("[BlackMarket] possibleItems is empty. Add InventoryItemData assets in Inspector.");
            return;
        }

        Debug.Log("=== NOVÁ BLACKMARKET NABÍDKA ===");

        for (int i = 0; i < offersPerRefresh; i++)
        {
            var picked = possibleItems[Random.Range(0, possibleItems.Count)];
            if (picked == null) continue;

            MarketOffer offer = new MarketOffer
            {
                itemData = picked,
                partName = string.IsNullOrWhiteSpace(picked.displayName) ? picked.name : picked.displayName,
                price = Random.Range(500, 3000),
                scamRisk = Random.Range(0.05f, 0.35f)
            };

            currentOffers.Add(offer);
            Debug.Log($"[{currentOffers.Count}] {offer.partName} | Cena: ${offer.price} | Scam risk: {offer.scamRisk:P0}");
        }

        Debug.Log($"[Cash] ${playerCash}");
        Debug.Log("Pro nákup použij tlačítka v Shop UI.");
    }

    // Hlavní nákup logika (volaná pouze z UI)
    private void TryBuyOffer(int index)
    {
        if (index < 0 || index >= currentOffers.Count)
        {
            Debug.LogWarning("Neplatný index nabídky.");
            return;
        }

        MarketOffer offer = currentOffers[index];
        if (offer == null || offer.itemData == null)
        {
            Debug.LogWarning("Nabídka nemá validní itemData.");
            return;
        }

        if (playerCash < offer.price)
        {
            Debug.LogWarning($"Nedostatek peněz na {offer.partName}. Potřebuješ ${offer.price}, máš ${playerCash}");
            return;
        }

        if (inventoryV2 == null)
        {
            Debug.LogError("BlackMarketManager: missing PlayerInventoryV2 reference.");
            return;
        }

        // Zaplatíš vždy
        playerCash -= offer.price;

        // Scam roll
        float roll = Random.value;
        bool scammed = roll < offer.scamRisk;

        if (scammed)
        {
            Debug.LogWarning($"SCAM! Zaplatil jsi ${offer.price} za {offer.partName}, ale nic nedorazilo. (roll {roll:F2})");
        }
        else
        {
            inventoryV2.AddItem(
                data: offer.itemData,
                amount: 1,
                durability01: 1f,
                countryOfOrigin: "",
                source: "BlackMarket",
                boughtPrice: offer.price
            );

            Debug.Log($"ÚSPĚCH! Koupil jsi {offer.partName} za ${offer.price}. Zásilka dorazila do inventáře. (roll {roll:F2})");
        }

        Debug.Log($"[Cash] Zůstatek: ${playerCash}");
    }

    // ===== PUBLIC API pro UI =====
    public void BuyOfferFromUI(int index)
    {
        TryBuyOffer(index);
    }

    public string GetOffersDebugText()
    {
        if (currentOffers == null || currentOffers.Count == 0)
            return "No offers.";

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < currentOffers.Count; i++)
        {
            var o = currentOffers[i];
            sb.AppendLine($"{i}: {o.partName} | ${o.price} | risk {o.scamRisk:P0}");
        }
        return sb.ToString();
    }

    public void ForceRefreshOffers()
    {
        GenerateOffers();
        if (gameTime != null)
            lastRefreshDay = gameTime.day;
    }

    public void AddCash(int amount)
    {
        playerCash += amount;
        Debug.Log($"[Cash] +${amount} => ${playerCash}");
    }
}