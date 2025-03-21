using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int playerCurrency = 500;
    public int maxHealth = 100;
    private int _currentHealth;
    private bool _isLose = false;

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

    public int GetCurrentHealth() {
        return _currentHealth;
    }

    public bool IsLose() {
        return _isLose;
    }
}
