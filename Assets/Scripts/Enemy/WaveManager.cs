using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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

    private int _waveNumber = 1;
    public bool IsWaveInProgress { get; private set; } = false;

    [Button]
    public void StartWaves()
    {
        StartCoroutine(WaveLoop());
    }

    private IEnumerator WaveLoop()
    {
        while (!_gameManager.IsLose()) {
            IsWaveInProgress = true;
            int enemyCount = Random.Range(minEnemiesPerWave, maxEnemiesPerWave + 1);
            for (int i = 0; i < enemyCount; i++)
            {
                _enemySpawner.SpawnRandomEnemy(i);
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitUntil(() => _enemySpawner.AllEnemiesDead());
            IsWaveInProgress = false;
            _waveNumber++;

            yield return new WaitForSeconds(delayBetweenWaves);
        }
    }
}
