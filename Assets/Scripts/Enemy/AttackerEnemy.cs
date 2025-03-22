using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AttackerEnemy : BaseEnemy {
    private Transform _targetTower;
    private BaseTower _targetBaseTower;
    private float _attackTimer = 0f;
    private float _nextAttackTime = 0f;
    
    private ProjectilePoolManager _projectilePoolManager;
    
    public void SetPoolManager(ProjectilePoolManager pool) {
        _projectilePoolManager = pool;
    }
    protected override void Update()
    {
        if (_targetTower != null) {
            if (_targetTower.Equals(null)) {
                ClearTarget();
            }
            else {
                float dist = Vector3.Distance(transform.position, _targetTower.position);

                if (dist > GetEnemyData().attackRange) {
                    ClearTarget();
                }
                else {
                    RotateToTarget(_targetTower);
                    StopMovement();
                    HandleAttack();
                    return;
                }
            }
        }
        else {
            SearchForTower();
        }

        base.Update();
    }

    private void ClearTarget() {
        _targetTower = null;
        ResumeMovement();
    }

    private void HandleAttack() {
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _nextAttackTime) {
            FireProjectile();
            _nextAttackTime = 5f / Mathf.Max(GetEnemyData().attackSpeed, 0.01f);
            _attackTimer = 0f;
        }
    }
    
    private void FireProjectile()
    {
        if (_targetTower == null) return;
        IDamageable targetDamageable = _targetTower.GetComponent<IDamageable>();
        
        if(targetDamageable == null || _projectilePoolManager == null) return;
        
        GameObject projectile = _projectilePoolManager.GetProjectile();
        projectile.transform.position = transform.position;

        Projectile proj = projectile.GetComponent<Projectile>();
        proj.Initialize(targetDamageable, _targetTower, GetEnemyData().projectileSpeed, GetEnemyData().damage, _projectilePoolManager);
    }

    private void SearchForTower() {
        Collider[] hits = Physics.OverlapSphere(transform.position, GetEnemyData().attackRange);
        foreach (var hit in hits) {
            if (hit.TryGetComponent(out BaseTower tower)) {
                _targetTower = tower.transform;
                _targetBaseTower = tower;
                _targetBaseTower.OnTowerDestroyed += HandleTowerDestroyed;
                StopMovement();
                break;
            }
        }
    }
    
    private void HandleTowerDestroyed(BaseTower tower)
    {
        if (tower == _targetBaseTower)
        {
            _targetTower = null;
            _targetBaseTower = null;
            ResumeMovement();
        }
    }
}
