using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AttackerEnemy : BaseEnemy {
    // Current target tower (as Transform and BaseTower reference)
    private Transform _targetTower;
    private BaseTower _targetBaseTower;
    // Attack cooldown system
    private float _attackTimer = 0f;
    private float _attackCooldown = 0f;
    
    private ProjectilePoolManager _projectilePoolManager;
    
    public void SetPoolManager(ProjectilePoolManager pool) {
        _projectilePoolManager = pool;
    }
    
    // Looks for towers in range to initiate attack
    protected override void OnMovingUpdate()
    {
        SearchForTower();
    }
    
    //Rotates towards the target tower and handles timed projectile attacks
    protected override void OnAttackingUpdate()
    {
        if (_targetTower == null) return;
        RotateToTarget(_targetTower);
        GetEnemyNavMesh.isStopped = true;
        GetEnemyNavMesh.velocity = Vector3.zero;
        HandleAttack();
    }

    // Handles attack timing using cooldown based on attack speed
    private void HandleAttack()
    {
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _attackCooldown) {
            FireProjectile();
            _attackCooldown = 5f / Mathf.Max(GetEnemyData().attackSpeed, 0.01f);
            _attackTimer = 0f;
        }
    }
    
    
    // Fires a projectile towards the current target tower. Uses the projectile pool manager for instantiation.
    private void FireProjectile()
    {
        if (_targetTower == null) return;
        IDamageable targetDamageable = _targetTower.GetComponent<IDamageable>();
        
        if(targetDamageable == null || _projectilePoolManager == null) return;
        
        GameObject projectile = _projectilePoolManager.GetPooledProjectile();
        projectile.transform.position = transform.position;

        Projectile proj = projectile.GetComponent<Projectile>();
        proj.Initialize(targetDamageable, _targetTower, GetEnemyData().projectileSpeed, GetEnemyData().damage, _projectilePoolManager);
    }

    
    // Checks for towers in range using OverlapSphere. If found, sets the tower as the attack target and changes state to Attacking.
    private void SearchForTower() {
        Collider[] hits = Physics.OverlapSphere(transform.position, GetEnemyData().attackRange);
        foreach (var hit in hits) {
            if (hit.TryGetComponent(out BaseTower tower)) {
                _targetTower = tower.transform;
                _targetBaseTower = tower;
                if (_targetBaseTower != null)
                {
                    _targetBaseTower.OnTowerDestroyed += HandleTowerDestroyed;
                }
                ChangeState(EnemyState.Attacking);
                break;
            }
        }
    }
    
    
    //Called when the targeted tower is destroyed.Clears current tower references and resumes movement.
    private void HandleTowerDestroyed(BaseTower tower)
    {
        if (tower == _targetBaseTower)
        {
            _targetTower = null;
            _targetBaseTower = null;
            ChangeState(EnemyState.Moving);
        }
    }
    
}
