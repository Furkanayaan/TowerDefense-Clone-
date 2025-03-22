using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Zenject;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Properties")]
    [Inject] private EnemySpawner _enemySpawner;

    [Inject] private GameManager _gameManager;
    public float delayBetweenWaves = 5f;
    public int minEnemiesPerWave = 3;
    public int maxEnemiesPerWave = 5;

    private int _waveNumber = 0;
    [ShowInInspector]public bool IsWaveInProgress { get; private set; } = false;
    
    private enum WaveState { Empty, WaitingToStart, Spawning, WaitingForClear, Cooldown }

    [SerializeField] private WaveState _state = WaveState.Empty;
    private bool emptyState = false;
    private float _nextWaveTime;
    private int _spawnedEnemyCount;
    private float _spawnInterval = 0.1f;
    private float _spawnTimer;

    private int _currentEnemyCount;

    public void InitStartWave() {
        _state = WaveState.Cooldown;
        _nextWaveTime = delayBetweenWaves;
        IsWaveInProgress = false;
    }

    public void StartWaves() {
        _currentEnemyCount = Random.Range(minEnemiesPerWave, maxEnemiesPerWave + 1);
        _spawnedEnemyCount = 0;
        _spawnTimer = 0f;
        _state = WaveState.Spawning;
    }
    
    //ToDo : state icindekilerini fonksiyon yap
    private void Update()
    {
        if (_gameManager.IsLose()) return;

        switch (_state) {
            case WaveState.Empty:
                emptyState = true;
                break;
            case WaveState.WaitingToStart:
                StartWaves();
                break;

            case WaveState.Spawning:
                IsWaveInProgress = true;
                _spawnTimer -= Time.deltaTime;
                if (_spawnTimer <= 0f && _spawnedEnemyCount < _currentEnemyCount) {
                    _enemySpawner.SpawnRandomEnemy(_spawnedEnemyCount);
                    _spawnedEnemyCount++;
                    _spawnTimer = _spawnInterval;
                }

                if (_spawnedEnemyCount >= _currentEnemyCount) {
                    _waveNumber++;
                    _state = WaveState.WaitingForClear;
                }
                break;

            case WaveState.WaitingForClear:
                if (_enemySpawner.AllEnemiesDead()) {
                    _state = WaveState.Cooldown;
                    _nextWaveTime = delayBetweenWaves;
                    IsWaveInProgress = false;
                }
                break;

            case WaveState.Cooldown:
                emptyState = false;
                _nextWaveTime -= Time.deltaTime;
                if (_nextWaveTime <= 0f) {
                    _state = WaveState.WaitingToStart;
                }
                break;
        }
    }

    public void SetNextWaveSec(TextMeshProUGUI nextWaveText) {
        if (IsWaveInProgress || emptyState) {
            nextWaveText.gameObject.SetActive(false);
            return;
        }
        nextWaveText.gameObject.SetActive(true);
        int nextWaveSec = Mathf.CeilToInt(_nextWaveTime);
        nextWaveText.text = "Next wave in " + nextWaveSec;

    }
    

    public int TotalWaveCount() {
        return _waveNumber;
    }
}
