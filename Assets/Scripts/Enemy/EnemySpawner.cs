using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemySpawner : MonoBehaviour {
    
    [Inject] private EnemyRoadPoints _enemyRoadPoints;
    [Header("Enemy Prefab")]
    [SerializeField] private GameObject runnerPrefab;
    [SerializeField] private GameObject attackerPrefab;
    

    private List<BaseEnemy> activeEnemies = new();

    public void SpawnRandomEnemy(int offsetIndex = 0)
    {
        GameObject prefab = Random.value < 0.5f ? runnerPrefab : attackerPrefab;
        Vector3 offset = new Vector3(offsetIndex * 1.5f, 0, 0);
        Vector3 spawnPos = _enemyRoadPoints.GetInitTransform().position + offset;

        GameObject enemyGO = Instantiate(prefab, spawnPos, Quaternion.identity);
        BaseEnemy enemy = enemyGO.GetComponent<BaseEnemy>();
        enemy.Initialize(_enemyRoadPoints.GetTargetTransform());
        activeEnemies.Add(enemy);
        enemy.OnDeath += () => activeEnemies.Remove(enemy);
    }

    public bool AllEnemiesDead()
    {
        return activeEnemies.Count == 0;
    }
}
