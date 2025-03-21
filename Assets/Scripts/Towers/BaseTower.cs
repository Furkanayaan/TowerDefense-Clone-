using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class BaseTower : MonoBehaviour, IDamageable, IHealth {
    [Inject] protected EnemySpawner _enemySpawner;
    [Inject] protected ProjectilePoolManager _projectilePoolManager;
    protected float _damage;
    private float _attackSpeed;
    private float _attackRange;
    protected float _projectileSpeed;
    private TowerData _towerData;
    
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

    public TowerData Data() {
        return _towerData;
    }
    
    private IEnumerator AttackRoutine() {
        while (true) {
            Attack();
            yield return new WaitForSeconds(5f / Mathf.Max(_attackSpeed, 0.01f));
        }
    }

    protected Transform FindNearestEnemyInRange() {
        
        List<BaseEnemy> enemies = _enemySpawner.AllEnemies();
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < enemies.Count; i++)
        {
            BaseEnemy enemy = enemies[i];
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance <= _attackRange && distance < minDistance)
            {
                closest = enemy.transform;
                minDistance = distance;
            }
        }
        
        return closest;
    }

    protected abstract void Attack();


    
    public void TakeDamage(float amount) {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0) {
            Destroy(gameObject);
        }
    }
}
