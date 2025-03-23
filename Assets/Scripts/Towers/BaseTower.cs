using System;
using System.Collections;
using System.Collections.Generic;
using ObjectsType;
using UnityEngine;
using Zenject;

public abstract class BaseTower : MonoBehaviour, IDamageable {
    [Inject] protected EnemySpawner _enemySpawner;
    [Inject] protected ProjectilePoolManager _projectilePoolManager;
    [Inject] private TowerPlacementManager _towerPlacementManager;
    
    protected float _damage;
    protected float _projectileSpeed;
    
    private float _attackSpeed;
    private float _attackRange;
    
    private TowerData _towerData;
    private float _attackTimer;
    // Triggered when tower is destroyed
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
        _attackTimer = 0f;
    }

    private void Update()
    {
        AttackWhenReady();
    }

    
    // Increments timer and triggers attack if cooldown is met
    private void AttackWhenReady()
    {
        if (!CanAttack()) return;

        _attackTimer += Time.deltaTime;

        if (_attackTimer >= GetCooldown())
        {
            Attack();
            _attackTimer = 0f;
        }
    }
    
    // Calculates attack cooldown based on tower's attack speed
    private float GetCooldown()
    {
        return 5f / Mathf.Max(_attackSpeed, 0.01f);
    }
    
    // Returns whether the tower is alive and has valid data
    private bool CanAttack()
    {
        return CurrentHealth > 0 && _towerData != null;
    }

    private void OnDestroy() {
        OnTowerDestroyed?.Invoke(this);
    }

    public TowerData Data() {
        return _towerData;
    }


    
    // Finds the best target: attacker enemies with lowest HP, otherwise closest enemy
    protected BaseEnemy FindBestEnemyInRange() {
        
        List<BaseEnemy> enemies = _enemySpawner.AllEnemies();
        BaseEnemy chosen = null;

        float minDistance = Mathf.Infinity;
        float lowestHp = Mathf.Infinity;

        for (int i = 0; i < enemies.Count; i++)
        {
            BaseEnemy enemy = enemies[i];
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance > _attackRange) continue;

            if (enemy.GetEnemyData().type == ObjectTypes.AttackerEnemy)
            {
                if (enemy.CurrentHealth < lowestHp)
                {
                    chosen = enemy;
                    lowestHp = enemy.CurrentHealth;
                }
            }
            else if (chosen == null && distance < minDistance)
            {
                chosen = enemy;
                minDistance = distance;
            }
        }

        return chosen;
    }

    // Abstract method to define specific tower attack behavior
    protected abstract void Attack();


    
    // Reduces health and destroys the tower when it dies
    public void TakeDamage(float amount) {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0) {
            _towerPlacementManager.RemoveTowerFromList(transform);
            Destroy(gameObject);
        }
    }
}
