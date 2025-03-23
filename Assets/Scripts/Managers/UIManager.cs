using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class UIManager : MonoBehaviour
{
    [Inject] private GridManager _gridManager;
    [Inject] private TowerPlacementManager _towerPlacementManager;
    [Inject] private GameManager _gameManager;
    [Inject] private WaveManager _waveManager;

    private TowerData _selectedTower;
    private bool _hasShownGameOverUI;

    // UI elements
    public TowerData[] availableTowers;
    public Image healthFill;
    public GameObject failUI;
    
    public TextMeshProUGUI[] availableTowersCost;
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI totalTowerCount;
    public TextMeshProUGUI totalWaveCount;
    public TextMeshProUGUI nextWaveSec;
    public TextMeshProUGUI addCellCost;
    public TextMeshProUGUI nextWaveText;
    public float fillingBarSpeed = 10f;
    public Button[] towerButtons;
    public Button deleteButton;
    public Button addCellButton;
    

    // Called on start, subscribe to events and initialize UI
    private void Start()
    {
        _hasShownGameOverUI = false;
        UpdateTowerCostTexts();
        addCellCost.text = "Cost : " + _gridManager.cellCost;

        _gameManager.OnCurrencyChanged += amount =>
        {
            UpdateCurrencyText(amount);
            ChangeButtonsInteractable();
        };
        _gameManager.OnPlayerLose += () =>
        {
            ShowGameOverUI();
            ChangeButtonsInteractable();
        };
        _towerPlacementManager.OnTowerCountChanged += amount =>
        {
            UpdateTowerText(amount);
            ChangeButtonsInteractable();
        };

        _waveManager.OnNextWaveCountdownChanged += UpdateWaveCountdownText;
        _waveManager.OnWaveCountChanged += UpdateWaveText;

        _waveManager.OnWaveStart += ChangeButtonsInteractable;

        _towerPlacementManager.OnSelectedDataChanged += ChangeButtonsInteractable;

        _gridManager.OnChangedCell += ChangeButtonsInteractable;

        _towerPlacementManager.OnIsDeletingChanged += ChangeButtonsInteractable;

    }

    // Updates currency display
    private void UpdateCurrencyText(int amount)
    {
        currencyText.text = "X" + amount;
    }

    // Displays game over UI once
    private void ShowGameOverUI()
    {
        if (!_hasShownGameOverUI)
        {
            failUI.SetActive(true);
            _hasShownGameOverUI = true;
        }
    }

    // Updates total tower count text
    private void UpdateTowerText(int amount)
    {
        totalTowerCount.text = "Tower Count : " + amount;
    }

    // Updates wave count text
    private void UpdateWaveText(int amount)
    {
        totalWaveCount.text = "Wave Count : " + amount;
    }
    
    // Initializes tower cost labels in the UI
    private void UpdateTowerCostTexts()
    {
        for (int i = 0; i < availableTowersCost.Length; i++)
        {
            int cost = availableTowers[i].cost;
            availableTowersCost[i].text = "Cost : " + cost;
        }
    }
    
    // Updates the next wave countdown text based on event
    private void UpdateWaveCountdownText(int sec)
    {
        if (_waveManager.IsWaveInProgress || sec <= 0)
        {
            nextWaveSec.gameObject.SetActive(false);
            return;
        }

        nextWaveSec.gameObject.SetActive(true);
        nextWaveSec.text = "Next wave in " + sec;
    }

    private void Update()
    {
        UpdateHealthBar();
    }
    
    // Smoothly updates the health bar fill
    private void UpdateHealthBar()
    {
        healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount,
            (float)_gameManager.GetCurrentHealth() / _gameManager.maxHealth, fillingBarSpeed * Time.deltaTime);
    }

    // Unsubscribes from events to prevent memory leaks
    private void OnDestroy()
    {
        _gameManager.OnCurrencyChanged -= UpdateCurrencyText;
        _gameManager.OnPlayerLose -= ShowGameOverUI;
        _towerPlacementManager.OnTowerCountChanged -= UpdateTowerText;
        _waveManager.OnWaveCountChanged -= UpdateWaveText;
        _waveManager.OnNextWaveCountdownChanged -= UpdateWaveCountdownText;
        
        _waveManager.OnWaveStart -= ChangeButtonsInteractable;

        _towerPlacementManager.OnSelectedDataChanged -= ChangeButtonsInteractable;

        _gridManager.OnChangedCell -= ChangeButtonsInteractable;

        _towerPlacementManager.OnIsDeletingChanged -= ChangeButtonsInteractable;
    }

    // Called when a tower button is clicked
    public void SelectTower(int index)
    {
        if (!towerButtons[index].interactable) return;

        if (index >= 0 && index < availableTowers.Length && _gameManager.TrySpendCurrency(availableTowers[index].cost))
        {
            _towerPlacementManager.SetSelectedTower(availableTowers[index]);
        }
    }
    
    // Enables/disables all UI buttons based on game state
    private void ChangeButtonsInteractable()
    {
        bool commonControl = !_waveManager.IsWaveInProgress && !_hasShownGameOverUI && _towerPlacementManager.GetSelectedData() == null && !_towerPlacementManager.ReturnDeleting();
        bool specifyForTowersButtons =  !_towerPlacementManager.AreAllCellsFully();
        bool specifyForDeleteButton = _towerPlacementManager.IsAnyTowerOnCell();
        bool specifyForCellButton = _gameManager.playerCurrency >= _gridManager.cellCost;

        addCellButton.interactable = commonControl && specifyForCellButton;
        deleteButton.interactable = commonControl && specifyForDeleteButton;
        for (int i = 0; i < towerButtons.Length; i++)
        {
            bool canAfford = _gameManager.playerCurrency >= availableTowers[i].cost;
            
            towerButtons[i].interactable = canAfford && commonControl && specifyForTowersButtons;
        }
    }

    

    // Reloads the current scene
    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}