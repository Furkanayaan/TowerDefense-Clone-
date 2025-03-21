using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class UIManager : MonoBehaviour
{
    [Inject] private GridManager _gridManager;
    [Inject] private TowerPlacementManager _towerPlacementManager;
    [Inject] private GameManager _gameManager;

    public TowerData[] availableTowers; // Inspector'dan eklenecek kuleler

    private TowerData selectedTower;
    public TextMeshProUGUI currencyText;

    private void Update() {
        currencyText.text = _gameManager.playerCurrency.ToString();
    }

    public void SelectTower(int index)
    {
        if (index >= 0 && index < availableTowers.Length && _gameManager.TrySpendCurrency(availableTowers[index].cost))
        {
            _towerPlacementManager.SetSelectedTower(availableTowers[index]);
        }
    }

    public void AddCell() {
        _gridManager.cellCount[_gridManager.rowCount]++;
        if (_gridManager.cellCount[_gridManager.rowCount] >= _gridManager.maksCellOnRow) {
            _gridManager.rowCount++;
            _gridManager.cellCount.Add(0);
        }
        _gridManager.GenerateGrid();
        
    }
}
