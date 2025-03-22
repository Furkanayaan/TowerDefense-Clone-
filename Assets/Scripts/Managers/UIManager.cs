using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class UIManager : MonoBehaviour
{
    [Inject] private GridManager _gridManager;
    [Inject] private TowerPlacementManager _towerPlacementManager;
    [Inject] private GameManager _gameManager;
    [Inject] private WaveManager _waveManager;
    
    private TowerData _selectedTower;
    private bool _isLoseControl;

    public TowerData[] availableTowers;
    public Image healthFill;
    public GameObject failUI;
    public int cellCost;
    public TextMeshProUGUI[] availableTowersCost;
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI totalTowerCount;
    public TextMeshProUGUI totalWaveCount;
    public TextMeshProUGUI nextWaveSec;
    public TextMeshProUGUI addCellCost;
    

    private void Start()
    {
        _isLoseControl = false;
        TowerCostText();
        addCellCost.text = "Cost : " + cellCost;
    }

    private void Update()
    {
        currencyText.text = "X" + _gameManager.playerCurrency;
        totalWaveCount.text = "Wave Count : " + _waveManager.TotalWaveCount();
        totalTowerCount.text = "Tower Count : " + _towerPlacementManager.TotalTower();
        _waveManager.SetNextWaveSec(nextWaveSec);
        HealthBar();
    }

    public void SelectTower(int index)
    {
        if (_waveManager.IsWaveInProgress || _isLoseControl) return;

        if (index >= 0 && index < availableTowers.Length && _gameManager.TrySpendCurrency(availableTowers[index].cost))
        {
            _towerPlacementManager.SetSelectedTower(availableTowers[index]);
        }
    }

    public void TowerCostText()
    {
        for (int i = 0; i < availableTowersCost.Length; i++)
        {
            int cost = availableTowers[i].cost;
            availableTowersCost[i].text = "Cost : " + cost;
        }
    }

    public void AddCell()
    {
        if (_waveManager.IsWaveInProgress || _isLoseControl) return;

        if (_gameManager.TrySpendCurrency(cellCost))
        {
            _gridManager.cellCount[_gridManager.rowCount]++;
            if (_gridManager.cellCount[_gridManager.rowCount] >= _gridManager.maksCellOnRow)
            {
                _gridManager.rowCount++;
                _gridManager.cellCount.Add(0);
            }

            _gridManager.GenerateGrid();
        }
    }

    public void HealthBar()
    {
        healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount,
            (float)_gameManager.GetCurrentHealth() / _gameManager.maxHealth, 10f * Time.deltaTime);
        if (_gameManager.IsLose() && !_isLoseControl)
        {
            failUI.SetActive(true);
            _isLoseControl = true;
        }
    }

    public void ClickRestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}