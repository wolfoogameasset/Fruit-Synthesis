using SCN.BinaryData;
using SCN.FirebaseLib.FA;
using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    public static Action OnCoinIncrease;
    public static Action OnCoinDecrease;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public int GetCurrentCoin()
    {
        return LocalDataManager.Instance.UserLocalData.CurrentCoins;
    }

    public void AddCoin(int coinValue, currencyEarnSourceType sourcetype)
    {
        LocalDataManager.Instance.UserLocalData.CurrentCoins += coinValue;
        LocalDataManager.Instance.SaveLocalData();
        OnCoinIncrease?.Invoke();
        //GAManagerCustom.Currency_earn(currencyType.coins.ToString(), coinValue, sourcetype.ToString());
    }
    public void RemoveCoin(int coinValue, currencySpendSource currencySpendSource, int id)
    {
        LocalDataManager.Instance.UserLocalData.CurrentCoins -= coinValue;
        LocalDataManager.Instance.SaveLocalData();
        OnCoinDecrease?.Invoke();
        //GAManagerCustom.Currency_spend(currencyType.coins.ToString(), coinValue, currencySpendSource.ToString() + "_" + id);
    }
}
public enum currencyType
{
    coins = 0
}
public enum currencyEarnSourceType
{
    hack = -1,
    merge = 0
}
public enum currencySpendSource
{
    hammer = 0,
}