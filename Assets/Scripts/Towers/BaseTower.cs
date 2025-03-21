using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTower : MonoBehaviour {
    protected float damage;
    protected float attackSpeed;
    protected float health;
    protected float attackRange;
    protected TowerData towerData;
    private Coroutine attackCoroutine;

    public void Initialize(TowerData data) {
        towerData = data;
        damage = data.damage;
        attackSpeed = data.attackSpeed;
        health = data.health;
        attackRange = data.attackRange;
        
        attackCoroutine = StartCoroutine(AttackRoutine());
    }

    public TowerData Data() {
        return towerData;
    }
    
    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            Attack();
            yield return new WaitForSeconds(attackSpeed);
        }
    }

    private void OnDrawGizmosSelected() {
        if (towerData != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,attackRange);
        }
    }

    protected abstract void Attack();
   
}
