using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    [Header("Money")]
    public int cash = 10000;

    public bool CanAfford(int amount)
    {
        return cash >= amount;
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0) return true;
        if (cash < amount) return false;

        cash -= amount;
        Debug.Log($"[Wallet] -${amount} => ${cash}");
        return true;
    }

    public void AddCash(int amount)
    {
        if (amount <= 0) return;
        cash += amount;
        Debug.Log($"[Wallet] +${amount} => ${cash}");
    }
}