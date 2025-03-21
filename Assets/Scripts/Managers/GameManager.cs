using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int playerCurrency = 500; // Başlangıç parası

    public bool TrySpendCurrency(int amount)
    {
        if (playerCurrency >= amount)
        {
            playerCurrency -= amount;
            Debug.Log($"Para harcandı: {amount}. Kalan bakiye: {playerCurrency}");
            return true;
        }
        Debug.Log("Yetersiz bakiye!");
        return false;
    }

    [Button]
    public void AddCurrency(int amount)
    {
        playerCurrency += amount;
        Debug.Log($"Para kazanıldı: {amount}. Toplam bakiye: {playerCurrency}");
    }
}
