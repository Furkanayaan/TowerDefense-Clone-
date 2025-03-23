using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Properties")] [Inject] private EnemySpawner _enemySpawner;

    [Inject] private GameManager _gameManager;
    // Time between waves
    public float delayBetweenWaves = 5f;
    // Range for number of enemies per wave
    public int minEnemiesPerWave = 3;
    public int maxEnemiesPerWave = 5;
    // Current wave number
    private int _waveNumber = 0;
    // Flag to check if wave is active
    [ShowInInspector] public bool IsWaveInProgress { get; private set; } = false;

    // Enum representing the current state of the wave
    private enum WaveState
    {
        Empty,
        WaitingToStart,
        Spawning,
        WaitingForClear,
        Cooldown
    }

    private WaveState _state = WaveState.Empty;
    // Time left for next wave
    private float _nextWaveTime;
    // Track how many enemies have been spawned
    private int _spawnedEnemyCount;
    // Time between individual enemy spawns
    public float spawnInterval = 0.1f;
    private float _spawnTimer;

    // Number of enemies to spawn in current wave
    private int _currentEnemyCount;

    // Event fired when wave count changes (used for UI)
    public Action<int> OnWaveCountChanged;
    // Event fired during cooldown countdown (used for UI)
    public Action<int> OnNextWaveCountdownChanged;

    //Only checks the interactable state of the buttons.
    public Action OnWaveStart;

    private void Start()
    {
        OnWaveCountChanged?.Invoke(_waveNumber);
    }

    // Public method to begin first cooldown and prep wave
    public void InitStartWave()
    {
        _state = WaveState.Cooldown;
        _nextWaveTime = delayBetweenWaves;
        IsWaveInProgress = false;
        OnWaveStart?.Invoke();
    }

    
    // Main update loop to handle wave states
    private void Update()
    {
        if (_gameManager.IsLose()) return;
        switch (_state)
        {
            case WaveState.Empty:
                break;
            case WaveState.WaitingToStart:
                WaitingToStartState();
                break;

            case WaveState.Spawning:
                SpawningState();
                break;

            case WaveState.WaitingForClear:
                WaitingForClearState();
                break;

            case WaveState.Cooldown:
                CooldownState();
                break;
        }
    }
    
    // Setup enemy count and switch to spawning state
    private void WaitingToStartState()
    {
        _currentEnemyCount = Random.Range(minEnemiesPerWave, maxEnemiesPerWave + 1);
        _spawnedEnemyCount = 0;
        _spawnTimer = 0f;
        _state = WaveState.Spawning;
    }

    // Spawn enemies with intervals until the count is reached
    private void SpawningState()
    {
        IsWaveInProgress = true;
        OnWaveStart?.Invoke();
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0f && _spawnedEnemyCount < _currentEnemyCount)
        {
            _enemySpawner.SpawnRandomEnemy(_spawnedEnemyCount);
            _spawnedEnemyCount++;
            _spawnTimer = spawnInterval;
        }

        if (_spawnedEnemyCount >= _currentEnemyCount)
        {
            _waveNumber++;
            OnWaveCountChanged?.Invoke(_waveNumber);
            _state = WaveState.WaitingForClear;
        }
    }

    // Wait for all enemies to be cleared before cooldown
    private void WaitingForClearState()
    {
        if (_enemySpawner.AllEnemiesDeadOrReachBase()) 
        {
            _state = WaveState.Cooldown;
            _nextWaveTime = delayBetweenWaves;
            IsWaveInProgress = false;
            OnWaveStart?.Invoke();
        }
    }

    // Countdown before next wave begins, fires UI event
    private void CooldownState()
    {
        _nextWaveTime -= Time.deltaTime;
        int nextWaveSec = Mathf.CeilToInt(_nextWaveTime);
        OnNextWaveCountdownChanged?.Invoke(nextWaveSec);
        if (_nextWaveTime <= 0f)
        {
            _state = WaveState.WaitingToStart;
        }
    }
}