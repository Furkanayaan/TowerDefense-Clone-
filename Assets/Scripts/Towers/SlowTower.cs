using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTower : BaseTower
{
    // Called when the tower is ready to attack
    protected override void Attack() {
        // Find the best enemy to target (prefers low HP attackers)
        BaseEnemy target = FindBestEnemyInRange();
        if (target != null && target.gameObject.activeInHierarchy)
        {
            // Get a pooled projectile instance
            GameObject projectile = _projectilePoolManager.GetPooledProjectile();
            projectile.transform.position = transform.position;

            // Retrieve cached Projectile script (avoids GetComponent)
            Projectile proj = _projectilePoolManager.ReturnProjectileScript(projectile);
            if(proj == null) return;
            // Initialize the projectile with target data and fire
            proj.Initialize(target, target.transform, _projectileSpeed, _damage, _projectilePoolManager);
        }
    }
}
