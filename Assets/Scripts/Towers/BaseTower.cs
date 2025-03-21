using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class BaseTower : MonoBehaviour, IDamageable, IHealth {
    [Inject] protected EnemySpawner _enemySpawner;
    protected float _damage;
    private float _attackSpeed;
    private float _attackRange;
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

    protected BaseEnemy FindNearestEnemyInRange() {
        
        List<BaseEnemy> enemies = _enemySpawner.AllEnemies();
        BaseEnemy closest = null;
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < enemies.Count; i++)
        {
            BaseEnemy enemy = enemies[i];
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance <= _attackRange && distance < minDistance)
            {
                closest = enemy;
                minDistance = distance;
            }
        }
        Debug.Log("Closest: " + closest);

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
