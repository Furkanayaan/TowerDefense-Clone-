using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour {
    [Inject] private CurrencyPool _currencyPool;
    public int playerCurrency = 500;
    public int maxHealth = 100;
    private int _currentHealth;
    private bool _isLose = false;
    public Transform toTheGoldUI;

    private void Start() {
        _currentHealth = maxHealth;
    }

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

    public void LoseHealth(int amount) {
        _currentHealth -= amount;
        if (_currentHealth <= 0) {
            _isLose = true;
        }
    }

    public void GoldPoolToGo(int quantity, Vector3 currentPos) {
        _currencyPool.CurrencyAllocation(quantity, CurrencyPool.PoolType.Gold, toTheGoldUI, currentPos);
    }

    public int GetCurrentHealth() {
        return _currentHealth;
    }

    public bool IsLose() {
        return _isLose;
    }
}
