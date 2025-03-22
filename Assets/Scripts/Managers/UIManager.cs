using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class UIManager : MonoBehaviour {
    
    //ToDo : Genel kod kontrolu
    //ToDo : Grid sistemini duzeltme
    //ToDo : Dokumantasyon yazma
    
    [Inject] private GridManager _gridManager;
    [Inject] private TowerPlacementManager _towerPlacementManager;
    [Inject] private GameManager _gameManager;
    [Inject] private WaveManager _waveManager;
 
    public TowerData[] availableTowers;

    private TowerData _selectedTower;
    public TextMeshProUGUI currencyText;

    public Image healthFill;
    public GameObject failUI;
    public TextMeshProUGUI totalTowerCount;
    public TextMeshProUGUI totalWaveCount;
    public TextMeshProUGUI nextWaveSec;
    private bool _isLoseControl;

    private void Start() {
        _isLoseControl = false;
    }

    private void Update() {
        currencyText.text = "X" + _gameManager.playerCurrency;
        totalWaveCount.text = "Wave Count : " + _waveManager.TotalWaveCount();
        totalTowerCount.text = "Tower Count : " + _towerPlacementManager.TotalTower();
        _waveManager.SetNextWaveSec(nextWaveSec);
        HealthBar();
    }

    public void SelectTower(int index) {
        if(_waveManager.IsWaveInProgress || _isLoseControl) return;
        
        if (index >= 0 && index < availableTowers.Length && _gameManager.TrySpendCurrency(availableTowers[index].cost))
        {
            _towerPlacementManager.SetSelectedTower(availableTowers[index]);
        }
    }

    public void AddCell() {
        if(_waveManager.IsWaveInProgress|| _isLoseControl) return;
        
        _gridManager.cellCount[_gridManager.rowCount]++;
        if (_gridManager.cellCount[_gridManager.rowCount] >= _gridManager.maksCellOnRow) {
            _gridManager.rowCount++;
            _gridManager.cellCount.Add(0);
        }
        _gridManager.GenerateGrid();
        
    }

    public void HealthBar() {
        healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, (float)_gameManager.GetCurrentHealth() / _gameManager.maxHealth, 10f*Time.deltaTime);
        if (_gameManager.IsLose() && !_isLoseControl) {
            failUI.SetActive(true);
            _isLoseControl = true;
        }
    }
    
    public void ClickRestartButton() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
