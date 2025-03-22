using System;
using System.Collections;
using System.Collections.Generic;
using ObjectsType;
using UnityEngine;
using Zenject;

public abstract class BaseTower : MonoBehaviour, IDamageable, IHealth {
    [Inject] protected EnemySpawner _enemySpawner;
    [Inject] protected ProjectilePoolManager _projectilePoolManager;
    [Inject] private TowerPlacementManager _towerPlacementManager;
    protected float _damage;
    private float _attackSpeed;
    private float _attackRange;
    protected float _projectileSpeed;
    private TowerData _towerData;
    public Action<BaseTower> OnTowerDestroyed;
    
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }

    public void Initialize(TowerData data) {
        _towerData = data;
        _damage = data.damage;
        _attackSpeed = data.attackSpeed;
        MaxHealth = data.health;
        CurrentHealth = MaxHealth;
        _attackRange = data.attackRange;
        _projectileSpeed = data.projectileSpeed;
        StartCoroutine(AttackRoutine());
    }

    private void OnDestroy() {
        OnTowerDestroyed?.Invoke(this);
    }

    public TowerData Data() {
        return _towerData;
    }
    
    private IEnumerator AttackRoutine() {
        while (true) {
            Attack();
            yield return new WaitForSeconds(5f / Mathf.Max(_attackSpeed, 0.01f));
        }
    }

    //ToDo : tek for ile coz en dusuk hpli olan dusman listenin sonunda olsun
    protected Transform FindBestEnemyInRange() {
        
        List<BaseEnemy> enemies = _enemySpawner.AllEnemies();
        List<BaseEnemy> attackersInRange = new();
        
        Transform targetEnemy = null;
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < enemies.Count; i++)
        {
            BaseEnemy enemy = enemies[i];
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if(distance > _attackRange) continue;

            if (enemy.GetEnemyData().type == ObjectTypes.AttackerEnemy) {
                attackersInRange.Add(enemy);
            }

            if (distance < minDistance) {
                targetEnemy = enemy.transform;
                minDistance = distance;
            }
        }

        float refHealth = Mathf.Infinity;
        if (attackersInRange.Count > 0) {
            for (int i = 0; i < attackersInRange.Count; i++) {
                float getHealth = attackersInRange[i].CurrentHealth;
                if (getHealth <= refHealth) {
                    targetEnemy = attackersInRange[i].transform;
                    refHealth = getHealth;
                }
            }
        }
        
        return targetEnemy;
    }

    protected abstract void Attack();


    
    public void TakeDamage(float amount) {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0) {
            _towerPlacementManager.RemoveTowerFromList(transform);
            Destroy(gameObject);
        }
    }
}
