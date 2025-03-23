using System;
using System.Collections.Generic;
using ObjectsType;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Inject] private EnemyRoadPoints _enemyRoadPoints;
    [Inject] private ProjectilePoolManager _projectilePoolManager;
    [Inject] private GameManager _gameManager;

    [Serializable]
    public struct EnemyConfig
    {
        public ObjectTypes type; // Type of the enemy
        public GameObject prefab; // Prefab reference
        public Transform activeParent; // Parent transform for active enemies
        public Transform deactiveParent; // Parent transform for pooled (inactive) enemies
    }

    [Header("Enemy Configuration")]
    [SerializeField] List<EnemyConfig> enemyConfigs; // List of enemy configurations
    [SerializeField] private int preWarmCount = 10; // Number of enemies to pre-instantiate per type

    private Dictionary<ObjectTypes, EnemyConfig> _enemyConfigMap = new(); // Config lookup by type
    private Dictionary<ObjectTypes, Queue<(GameObject, BaseEnemy)>> _pools = new(); // Object pool per enemy type
    private List<BaseEnemy> _activeEnemies = new(); // Currently active enemies on scene

    private void Start()
    {
        for (int i = 0; i < enemyConfigs.Count; i++)
        {
            // Map config by type
            _enemyConfigMap[enemyConfigs[i].type] = enemyConfigs[i];
            // Create pool
            _pools[enemyConfigs[i].type] = new Queue<(GameObject, BaseEnemy)>();
            // Pre-instantiate enemies
            PrewarmPool(enemyConfigs[i]);
        }
    }

    private void PrewarmPool(EnemyConfig config)
    {
        for (int i = 0; i < preWarmCount; i++)
        {
            // Instantiate under deactive parent
            GameObject enemyGameObject = Instantiate(config.prefab, config.deactiveParent);
            // Get BaseEnemy component
            BaseEnemy enemy = enemyGameObject.GetComponent<BaseEnemy>();
            // Add to pool
            _pools[config.type].Enqueue((enemyGameObject,enemy));
        }
    }

    public void SpawnRandomEnemy(int offsetIndex = 0)
    {
        // Random enemy type
        ObjectTypes type = Random.value < 0.5f ? ObjectTypes.RunnerEnemy : ObjectTypes.AttackerEnemy;
        EnemyConfig config = _enemyConfigMap[type];

        // Offset to avoid overlap
        Vector3 offset = new Vector3(offsetIndex * 1.5f, 0, 0);
        // Final spawn position
        Vector3 spawnPos = _enemyRoadPoints.GetInitTransform().position + offset;

        // Get from pool
        (GameObject enemyGameObject, BaseEnemy enemy) = GetFromPool(type);
        enemyGameObject.transform.position = spawnPos;
        // Activate in scene
        enemyGameObject.transform.SetParent(config.activeParent);

        enemy.Initialize(_enemyRoadPoints.GetTargetTransform());

        if (type == ObjectTypes.AttackerEnemy && enemy is AttackerEnemy attacker)
        {
            // Set projectile pool for attackers
            attacker.SetPoolManager(_projectilePoolManager);
        }

        // Track enemy
        _activeEnemies.Add(enemy);
        // Handle death
        enemy.OnDeath += () => { RemoveEnemy(enemy, true); };
        // Handle base reach
        enemy.OnFinish += () => { RemoveEnemy(enemy, false); };
    }

    private (GameObject, BaseEnemy) GetFromPool(ObjectTypes type)
    {
        var pool = _pools[type];
        if (pool.Count > 0)
            return pool.Dequeue(); // Reuse if available

        GameObject prefab = _enemyConfigMap[type].prefab; // Fallback instantiate
        GameObject enemyGameObject = Instantiate(prefab);
        BaseEnemy enemy = enemyGameObject.GetComponent<BaseEnemy>();
        return (enemyGameObject, enemy);
    }

    private void ReturnToPool(BaseEnemy enemy)
    {
        // Determine type
        ObjectTypes type = enemy.GetEnemyData().type; 
        GameObject enemyGameObject = enemy.gameObject;
        EnemyConfig config = _enemyConfigMap[type];
        // Move back to deactive parent
        enemyGameObject.transform.SetParent(config.deactiveParent);
        // Return to pool
        _pools[type].Enqueue((enemyGameObject, enemy));
    }

    private void RemoveEnemy(BaseEnemy enemy, bool bDeath)
    {
        // Disable movement
        enemy.GetEnemyNavMesh.enabled = false;
        // Remove from active list
        _activeEnemies.Remove(enemy);
        // Return to pool
        ReturnToPool(enemy);

        if (bDeath)
            RewardPlayer(enemy); // Reward player
        else
            DamagePlayer(enemy); // Apply base damage
    }

    private void RewardPlayer(BaseEnemy enemy)
    {
        int reward = enemy.GetEnemyData().rewardAmount;
        // Visual + reward logic
        _gameManager.GoldPoolToGo(reward, enemy.transform.position);
    }

    private void DamagePlayer(BaseEnemy enemy)
    {
        int damage = enemy.GetEnemyData().damageOnBase;
        // Reduce base health
        _gameManager.LoseHealth(damage);
    }

    public bool AllEnemiesDeadOrReachBase() => _activeEnemies.Count == 0;

    public List<BaseEnemy> AllEnemies() => _activeEnemies;
}
