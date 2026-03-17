using System;
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
        public string supplierName;
        public string countryOfOrigin;
        public string deliveryPoint;
        public int price;
        public float scamRisk;      // 0.0 - 1.0 (dodavatel tě ojebe)
        public float handoverRisk;  // 0.0 - 1.0 (riziko převzetí zásilky)
    }

    [Header("Reference")]
    public GameTimeManager gameTime;
    public PlayerInventoryV2 inventoryV2;
    public PlayerWallet wallet;

    [Header("Market Pool (InventoryItemData assets)")]
    public List<InventoryItemData> possibleItems = new List<InventoryItemData>();

    [Header("Market Settings")]
    public int refreshEveryDays = 3;
    public int offersPerRefresh = 4;

    private int lastRefreshDay = 1;
    private readonly List<MarketOffer> currentOffers = new List<MarketOffer>();
    private static readonly string[] DeliveryPoints = { "Dock North", "Dock South", "Airport Cargo" };
    private static readonly char[] NameSeparators = { ' ', '-', '_', '.', '/', '\\' };
    private const float AirportHandoverRiskMin = 0.12f;
    private const float AirportHandoverRiskMax = 0.25f;
    private const float DockHandoverRiskMin = 0.03f;
    private const float DockHandoverRiskMax = 0.12f;

    private static float GetScamRisk(MarketOffer offer) => Mathf.Clamp01(offer.scamRisk);
    private static float GetEffectiveHandoverRisk(MarketOffer offer) => Mathf.Clamp01(Mathf.Min(offer.handoverRisk, 1f - GetScamRisk(offer)));
    private static float GetTotalRisk(MarketOffer offer) => GetScamRisk(offer) + GetEffectiveHandoverRisk(offer);

    private static bool HasToken(string raw, string token)
    {
        if (string.IsNullOrWhiteSpace(raw) || string.IsNullOrWhiteSpace(token)) return false;
        string[] parts = raw.Split(NameSeparators, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < parts.Length; i++)
        {
            if (string.Equals(parts[i], token, StringComparison.OrdinalIgnoreCase)) return true;
        }

        return false;
    }

    void Start()
    {
        if (gameTime == null) gameTime = FindObjectOfType<GameTimeManager>();
        if (inventoryV2 == null) inventoryV2 = FindObjectOfType<PlayerInventoryV2>();
        if (wallet == null) wallet = FindObjectOfType<PlayerWallet>();

        GenerateOffers();
        lastRefreshDay = gameTime != null ? gameTime.day : 1;

        if (wallet != null)
            Debug.Log($"[Cash] Start cash: ${wallet.cash}");

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
                deliveryPoint = DeliveryPoints[Random.Range(0, DeliveryPoints.Length)]
            };

            string partNameLower = offer.partName.ToLowerInvariant();
            bool isBarrel = partNameLower.Contains("barrel");
            bool isMag = HasToken(partNameLower, "mag") || partNameLower.Contains("magazine");
            bool isBodyOrStock =
                HasToken(partNameLower, "body") ||
                partNameLower.Contains("receiver") ||
                HasToken(partNameLower, "stock");

            if (isBarrel)
            {
                offer.supplierName = "Levant Forge";
                offer.countryOfOrigin = "Israel";
                offer.price = Random.Range(1800, 3500);
                offer.scamRisk = Random.Range(0.10f, 0.28f);
            }
            else if (isMag)
            {
                offer.supplierName = "Volga Steel";
                offer.countryOfOrigin = "Russia";
                offer.price = Random.Range(700, 1700);
                offer.scamRisk = Random.Range(0.08f, 0.22f);
            }
            else if (isBodyOrStock)
            {
                offer.supplierName = "Baltic Frames";
                offer.countryOfOrigin = "Poland";
                offer.price = Random.Range(1200, 2600);
                offer.scamRisk = Random.Range(0.07f, 0.20f);
            }
            else
            {
                offer.supplierName = "Grey Broker";
                offer.countryOfOrigin = "Unknown";
                offer.price = Random.Range(500, 3000);
                offer.scamRisk = Random.Range(0.05f, 0.35f);
            }

            offer.handoverRisk = offer.deliveryPoint == "Airport Cargo"
                ? Random.Range(AirportHandoverRiskMin, AirportHandoverRiskMax)
                : Random.Range(DockHandoverRiskMin, DockHandoverRiskMax);

            currentOffers.Add(offer);
            float totalRisk = GetTotalRisk(offer);
            Debug.Log($"[{currentOffers.Count}] {offer.partName} | {offer.countryOfOrigin} | {offer.supplierName} | {offer.deliveryPoint} | Cena: ${offer.price} | Risk: {totalRisk:P0}");
        }

        if (wallet != null)
            Debug.Log($"[Cash] ${wallet.cash}");

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

        if (inventoryV2 == null)
        {
            Debug.LogError("BlackMarketManager: missing PlayerInventoryV2 reference.");
            return;
        }

        if (wallet == null)
        {
            Debug.LogError("BlackMarketManager: missing PlayerWallet reference.");
            return;
        }

        if (!wallet.CanAfford(offer.price))
        {
            Debug.LogWarning($"Nedostatek peněz na {offer.partName}. Potřebuješ ${offer.price}, máš ${wallet.cash}");
            return;
        }

        // Zaplatíš vždy
        wallet.TrySpend(offer.price);

        // Scam roll
        float roll = Random.value;
        float scamRisk = GetScamRisk(offer);
        float totalRisk = GetTotalRisk(offer);
        bool scammed = roll < scamRisk;
        bool failedHandover = roll >= scamRisk && roll < totalRisk;

        if (scammed)
        {
            Debug.LogWarning($"SCAM! Zaplatil jsi ${offer.price} za {offer.partName}, ale nic nedorazilo. (roll {roll:F2})");
        }
        else if (failedHandover)
        {
            Debug.LogWarning($"PŘEVZETÍ SELHALO! {offer.partName} ({offer.countryOfOrigin}) bylo zabaveno při vyzvednutí na {offer.deliveryPoint}. (roll {roll:F2})");
        }
        else
        {
            inventoryV2.AddItem(
                data: offer.itemData,
                amount: 1,
                durability01: 1f,
                countryOfOrigin: offer.countryOfOrigin,
                source: $"BlackMarket/{offer.supplierName}/{offer.deliveryPoint}",
                boughtPrice: offer.price
            );

            Debug.Log($"ÚSPĚCH! Koupil jsi {offer.partName} ({offer.countryOfOrigin}) od {offer.supplierName} za ${offer.price}. Zásilka dorazila přes {offer.deliveryPoint}. (roll {roll:F2})");
        }

        Debug.Log($"[Cash] Zůstatek: ${wallet.cash}");
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
            float totalRisk = GetTotalRisk(o);
            sb.AppendLine($"{i}: {o.partName} ({o.countryOfOrigin}) | {o.supplierName} | {o.deliveryPoint} | ${o.price} | risk {totalRisk:P0}");
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
        if (wallet == null)
        {
            Debug.LogError("BlackMarketManager: missing PlayerWallet reference.");
            return;
        }

        wallet.AddCash(amount);
    }
}
