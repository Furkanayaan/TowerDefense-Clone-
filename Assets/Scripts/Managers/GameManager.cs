using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject] private CurrencyPool _currencyPool;

    // Starting player currency
    public int playerCurrency = 500;

    // Max player health
    public int maxHealth = 100;

    // Current player health
    private int _currentHealth;

    // Lose state flag
    private bool _isLose = false;

    // Target position for currency UI animation
    public Transform toTheGoldUI;

    // Event triggered when currency changes
    public Action<int> OnCurrencyChanged;

    // Event triggered when player loses
    public Action OnPlayerLose;

    private void Start()
    {
        _currentHealth = maxHealth;
        // Notify UI of initial currency
        OnCurrencyChanged?.Invoke(playerCurrency);
    }

    public bool TrySpendCurrency(int amount)
    {
        // Attempt to spend currency; return true if successful
        if (playerCurrency >= amount)
        {
            playerCurrency -= amount;
            // Notify UI of change
            OnCurrencyChanged?.Invoke(playerCurrency);
            return true;
        }

        return false;
    }

    public void AddCurrency(int amount)
    {
        // Add currency and notify UI
        playerCurrency += amount;
        OnCurrencyChanged?.Invoke(playerCurrency);
    }

    public void LoseHealth(int amount)
    {
        if(_currentHealth <= 0) return;
        // Reduce player health and trigger lose event if needed
        _currentHealth -= amount;
        if (_currentHealth <= 0)
        {
            _isLose = true;
            OnPlayerLose?.Invoke();
        }
    }

    public void GoldPoolToGo(int quantity, Vector3 currentPos)
    {
        // Spawn floating gold animation from currentPos to UI
        _currencyPool.CurrencyAllocation(quantity, CurrencyPool.PoolType.Gold, toTheGoldUI, currentPos);
    }

    public int GetCurrentHealth()
    {
        return _currentHealth;
    }

    public bool IsLose()
    {
        return _isLose;
    }
}