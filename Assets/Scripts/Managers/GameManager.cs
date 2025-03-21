using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int playerCurrency = 500;

    public bool TrySpendCurrency(int amount) {
        if (playerCurrency >= amount) {
            playerCurrency -= amount;
            return true;
        }
        return false;
    }

    [Button]
    public void AddCurrency(int amount) {
        playerCurrency += amount;
    }
}
