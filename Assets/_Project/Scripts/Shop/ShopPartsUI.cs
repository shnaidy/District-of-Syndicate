using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPartsUI : MonoBehaviour
{
    [Header("Data")]
    public ShopPartDatabase database;

    [Header("External refs")]
    public PlayerInventoryV2 inventoryV2;
    public GunPartInventoryMap inventoryMap;
    public PlayerWallet wallet;

    [Header("Left column (part types)")]
    public Transform typesContainer;
    public Button typeButtonPrefab;

    [Header("Middle column (parts list)")]
    public Transform partsContainer;
    public Button partButtonPrefab;

    [Header("Right panel (detail)")]
    public TMP_Text nameText;
    public TMP_Text typeText;
    public TMP_Text priceText;
    public TMP_Text countryText;
    public TMP_Text descriptionText;
    public Image partImage;
    public Button buyButton;
    public TMP_Text buyButtonText;
    public TMP_Text cashText;

    [Header("Inventory UI (optional)")]
    public TMP_Text inventoryCountText;

    private readonly List<Button> spawnedTypeButtons = new();
    private readonly List<Button> spawnedPartButtons = new();

    private GunPart selectedPart;

    private void Start()
    {
        if (wallet == null) wallet = FindObjectOfType<PlayerWallet>();
        if (inventoryV2 == null) inventoryV2 = FindObjectOfType<PlayerInventoryV2>();
    }

    private void OnEnable()
    {
        BuildTypeButtons();
        ClearPartButtons();
        ClearDetail();
        RefreshCashUI();
        RefreshInventoryUI();
        RefreshBuyButtonUI();
    }

    private void BuildTypeButtons()
    {
        ClearButtons(spawnedTypeButtons);

        var allTypes = (GunPartType[])System.Enum.GetValues(typeof(GunPartType));
        foreach (var t in allTypes)
        {
            var btn = Instantiate(typeButtonPrefab, typesContainer);
            spawnedTypeButtons.Add(btn);

            var txt = btn.GetComponentInChildren<TMP_Text>();
            if (txt) txt.text = ToDisplayName(t);

            var capturedType = t;
            btn.onClick.AddListener(() =>
            {
                BuildPartsForType(capturedType);
                ClearDetail();
            });
        }
    }

    private void BuildPartsForType(GunPartType type)
    {
        ClearPartButtons();

        if (database == null || database.parts == null) return;

        foreach (var part in database.parts)
        {
            if (part == null) continue;
            if (part.partType != type) continue;

            var btn = Instantiate(partButtonPrefab, partsContainer);
            spawnedPartButtons.Add(btn);

            var txt = btn.GetComponentInChildren<TMP_Text>();
            if (txt) txt.text = GetShortPartName(part.partName);

            var capturedPart = part;
            btn.onClick.AddListener(() => ShowDetail(capturedPart));
        }
    }

    private void ShowDetail(GunPart part)
    {
        selectedPart = part;

        if (nameText) nameText.text = GetShortPartName(part.partName);
        if (typeText) typeText.text = $"Type: {ToDisplayName(part.partType)}";
        if (priceText) priceText.text = $"Price: ${part.price}";
        if (countryText) countryText.text = $"Origin: {part.countryOfOrigin}";
        if (descriptionText) descriptionText.text = part.description;
        if (partImage) partImage.sprite = part.image;

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(BuySelectedPart);
        }

        RefreshBuyButtonUI();
    }

    private void BuySelectedPart()
    {
        if (selectedPart == null) return;

        if (inventoryV2 == null)
        {
            Debug.LogWarning("[ShopPartsUI] inventoryV2 reference missing.");
            return;
        }

        if (inventoryMap == null)
        {
            Debug.LogWarning("[ShopPartsUI] inventoryMap reference missing.");
            return;
        }

        if (wallet == null)
        {
            Debug.LogWarning("[ShopPartsUI] wallet reference missing.");
            return;
        }

        if (!wallet.CanAfford(selectedPart.price))
        {
            Debug.Log("[Shop] Not enough cash.");
            RefreshBuyButtonUI();
            return;
        }

        if (!inventoryMap.TryGetItemData(selectedPart, out var itemData) || itemData == null)
        {
            Debug.LogWarning($"[ShopPartsUI] No InventoryItemData mapping for part: {selectedPart.partName}");
            return;
        }

        bool spent = wallet.TrySpend(selectedPart.price);
        if (!spent) return;

        inventoryV2.AddItem(
            data: itemData,
            amount: 1,
            durability01: 1f,
            countryOfOrigin: selectedPart.countryOfOrigin,
            source: "Shop",
            boughtPrice: selectedPart.price
        );

        Debug.Log($"[Shop] Bought: {selectedPart.partName} for ${selectedPart.price}");

        RefreshCashUI();
        RefreshInventoryUI();
        RefreshBuyButtonUI();
    }

    private void ClearPartButtons() => ClearButtons(spawnedPartButtons);

    private void ClearDetail()
    {
        selectedPart = null;
        if (nameText) nameText.text = "Select Part";
        if (typeText) typeText.text = "Type: -";
        if (priceText) priceText.text = "Price: -";
        if (countryText) countryText.text = "Origin: -";
        if (descriptionText) descriptionText.text = "";
        if (partImage) partImage.sprite = null;

        RefreshBuyButtonUI();
    }

    private void RefreshCashUI()
    {
        if (cashText == null) return;

        if (wallet == null) cashText.text = "Cash: -";
        else cashText.text = $"Cash: ${wallet.cash}";
    }

    private void RefreshInventoryUI()
    {
        if (inventoryCountText == null) return;

        if (inventoryV2 == null)
        {
            inventoryCountText.text = "Owned parts: -";
            return;
        }

        inventoryCountText.text = $"Owned parts: {inventoryV2.GetTotalItemCount()}";
    }

    private void RefreshBuyButtonUI()
    {
        bool hasSelection = selectedPart != null;
        bool canAfford = hasSelection && wallet != null && wallet.CanAfford(selectedPart.price);

        if (buyButton) buyButton.gameObject.SetActive(hasSelection);
        if (buyButton) buyButton.interactable = canAfford;

        if (buyButtonText)
        {
            if (!hasSelection) buyButtonText.text = "";
            else if (canAfford) buyButtonText.text = "BUY PART";
            else buyButtonText.text = "NOT ENOUGH CASH";
        }
    }

    private void ClearButtons(List<Button> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null) Destroy(list[i].gameObject);
        }
        list.Clear();
    }

    private string ToDisplayName(GunPartType t)
    {
        return t switch
        {
            GunPartType.BoltCarrierGroup => "BOLT CARRIER GROUP",
            GunPartType.ChargingHandle => "CHARGING HANDLE",
            GunPartType.LowerReceiver => "LOWER RECEIVER",
            GunPartType.PistolGrip => "PISTOL GRIP",
            _ => t.ToString().ToUpper()
        };
    }

    private string GetShortPartName(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName)) return "Part";
        if (rawName.StartsWith("AR-15 Barrel")) return "AR-15 Barrel";
        return rawName;
    }
}