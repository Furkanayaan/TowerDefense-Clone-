using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTower : BaseTower
{
    protected override void Attack() {
        BaseEnemy target = FindNearestEnemyInRange();
        if (target != null) {
            target.TakeDamage(_damage);
        }
    }
}
