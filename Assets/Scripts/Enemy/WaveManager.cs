using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Properties")]
    [Inject] private EnemySpawner _enemySpawner;
    [SerializeField] private float delayBetweenWaves = 5f;
    [SerializeField] private int minEnemiesPerWave = 3;
    [SerializeField] private int maxEnemiesPerWave = 5;

    private int waveNumber = 1;
    public bool IsWaveInProgress { get; private set; } = false;

    [Button]
    public void StartWaves()
    {
        StartCoroutine(WaveLoop());
    }

    private IEnumerator WaveLoop()
    {
        while (true) {
            IsWaveInProgress = true;
            int enemyCount = Random.Range(minEnemiesPerWave, maxEnemiesPerWave + 1);
            for (int i = 0; i < enemyCount; i++)
            {
                _enemySpawner.SpawnRandomEnemy(i);
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitUntil(() => _enemySpawner.AllEnemiesDead());
            IsWaveInProgress = false;
            waveNumber++;

            yield return new WaitForSeconds(delayBetweenWaves);
        }
    }
}
