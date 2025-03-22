using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;
using ObjectsType;
using UnityEngine.Serialization;

public class EnemySpawner : MonoBehaviour {
    
    [Inject] private EnemyRoadPoints _enemyRoadPoints;
    [Inject] private ProjectilePoolManager _projectilePoolManager;
    [Inject] private GameManager _gameManager;
    
    [Header("Enemy Prefab")]
    [SerializeField] private GameObject runnerPrefab;
    [SerializeField] private GameObject attackerPrefab;
    public int preWarmCount = 10;
    
    [Header("Parent References")]
    public Transform activeRunnerParent;
    public Transform deactiveRunnerParent;
    public Transform activeAttackerParent;
    public Transform deactiveAttackerParent;
    
    private Queue<GameObject> _runnerPool = new();
    private Queue<GameObject> _attackerPool = new();
    

    private List<BaseEnemy> _activeEnemies = new();

    private void Start() {
        PrewarmPool(runnerPrefab, _runnerPool, deactiveRunnerParent);
        PrewarmPool(attackerPrefab, _attackerPool, deactiveAttackerParent);
    }
    
    private void PrewarmPool(GameObject prefab, Queue<GameObject> pool, Transform parent)
    {
        for (int i = 0; i < preWarmCount; i++) {
            GameObject enemyGameObject = Instantiate(prefab, parent);
            pool.Enqueue(enemyGameObject);
        }
    }
    
    public GameObject GetFromPool(GameObject prefab, Transform activeParent) {
        GameObject enemyGameObject;
        ObjectTypes type = prefab.GetComponent<BaseEnemy>().GetEnemyData().type;
        
        Queue<GameObject> pool = type == ObjectTypes.RunnerEnemy ? _runnerPool : _attackerPool;

        if (pool.Count > 0)
            enemyGameObject = pool.Dequeue();
        
        else
            enemyGameObject = Instantiate(prefab);

        enemyGameObject.transform.SetParent(activeParent);
        
        return enemyGameObject;
    }
    
    public void ReturnToPool(GameObject enemyGameObject) {
        
        BaseEnemy baseEnemy = enemyGameObject.GetComponent<BaseEnemy>();
        if(baseEnemy == null) return;
        
        ObjectTypes type = baseEnemy.GetEnemyData().type;
        Queue<GameObject> pool = type == ObjectTypes.RunnerEnemy ? _runnerPool : _attackerPool;
        
        
        switch (baseEnemy.GetEnemyData().type) {
            case ObjectTypes.RunnerEnemy:
                enemyGameObject.transform.SetParent(deactiveRunnerParent);
                break;
            case ObjectTypes.AttackerEnemy:
                enemyGameObject.transform.SetParent(deactiveAttackerParent);
                break;
        }

        pool.Enqueue(enemyGameObject);
    }

    public void SpawnRandomEnemy(int offsetIndex = 0)
    {
        bool isRunner = Random.value < 0.5f;
        GameObject prefab = isRunner ? runnerPrefab : attackerPrefab;
        
        Transform activeParent = isRunner ? activeRunnerParent : activeAttackerParent;
        
        
        Vector3 offset = new Vector3(offsetIndex * 1.5f, 0, 0);
        Vector3 spawnPos = _enemyRoadPoints.GetInitTransform().position + offset;

        
        GameObject enemyGameObject = GetFromPool(prefab, activeParent);
        enemyGameObject.transform.position = spawnPos;
        
        BaseEnemy enemy = enemyGameObject.GetComponent<BaseEnemy>();
        enemy.Initialize(_enemyRoadPoints.GetTargetTransform());
        
        if (enemyGameObject.TryGetComponent<AttackerEnemy>(out var attackerEnemy)) {
            enemyGameObject.GetComponent<AttackerEnemy>().SetPoolManager(_projectilePoolManager);
        }
        _activeEnemies.Add(enemy);
        enemy.OnDeath += () => {
            RemoveEnemy(enemy, enemyGameObject, true);
        };
        enemy.OnFinish += () => {
            RemoveEnemy(enemy, enemyGameObject, false);
        };
    }

    public void RemoveEnemy(BaseEnemy enemy, GameObject enemyGameObject, bool bDeath)
    {
        enemy.GetEnemyNavMesh.enabled = false;
        _activeEnemies.Remove(enemy);
        ReturnToPool(enemyGameObject);
        if (bDeath) {
            int reward = enemy.GetEnemyData().rewardAmount;
            _gameManager.GoldPoolToGo(reward, enemyGameObject.transform.position);
            //_gameManager.AddCurrency(reward);
        }
        else {
            int damageBase = enemy.GetEnemyData().damageOnBase;
            _gameManager.LoseHealth(damageBase);
        }
    }

    public bool AllEnemiesDead()
    {
        return _activeEnemies.Count == 0;
    }

    public List<BaseEnemy> AllEnemies() {
        return _activeEnemies;
    }
}
