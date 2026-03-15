using UnityEngine;
using TMPro;

public class ShopUIBinder : MonoBehaviour
{
    public BlackMarketManager market;
    public TextMeshProUGUI offersText;
    public TextMeshProUGUI cashText;

    void Start()
    {
        if (market == null) market = FindObjectOfType<BlackMarketManager>();
        Refresh();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            Refresh();
    }

    public void BuyOffer(int index)
    {
        if (market == null) return;
        // BlackMarketManager.TryBuyOffer je private v tvém kódu -> uděláme public wrapper níž
        market.BuyOfferFromUI(index);
        Refresh();
    }

    public void Refresh()
    {
        if (market == null) return;

        if (cashText != null)
            cashText.text = $"Cash: ${market.playerCash}";

        if (offersText != null)
            offersText.text = market.GetOffersDebugText();
    }
}