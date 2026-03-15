using UnityEngine;
using TMPro;

public class ShopUIBinder : MonoBehaviour
{
    public BlackMarketManager market;
    public PlayerWallet wallet;
    public TextMeshProUGUI offersText;
    public TextMeshProUGUI cashText;

    void Start()
    {
        if (market == null) market = FindObjectOfType<BlackMarketManager>();
        if (wallet == null) wallet = FindObjectOfType<PlayerWallet>();
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
        market.BuyOfferFromUI(index);
        Refresh();
    }

    public void Refresh()
    {
        if (market == null) return;

        if (cashText != null && wallet != null)
            cashText.text = $"Cash: ${wallet.cash}";

        if (offersText != null)
            offersText.text = market.GetOffersDebugText();
    }
}