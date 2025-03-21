using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastTower : BaseTower
{
    protected override void Attack() {
        BaseEnemy target = FindNearestEnemyInRange();
        if (target != null) {
            target.TakeDamage(_damage);
        }
    }

    
}
