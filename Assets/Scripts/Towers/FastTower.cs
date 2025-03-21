using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastTower : BaseTower
{
    protected override void Attack() {
        Transform targetTransform = FindNearestEnemyInRange();
        if (targetTransform != null) {
            IDamageable targetDamageable = targetTransform.GetComponent<IDamageable>();
            if (targetDamageable != null) {
                GameObject projectile = _projectilePoolManager.GetProjectile();
                projectile.transform.position = transform.position;

                Projectile proj = projectile.GetComponent<Projectile>();
                proj.Initialize(targetDamageable, targetTransform, _projectileSpeed, _damage, _projectilePoolManager);
            }
        }
    }

    
}
