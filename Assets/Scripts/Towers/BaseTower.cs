using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTower : MonoBehaviour {
    private float _damage;
    private float _attackSpeed;
    private float _health;
    private float _attackRange;
    private TowerData _towerData;

    public void Initialize(TowerData data) {
        _towerData = data;
        _damage = data.damage;
        _attackSpeed = data.attackSpeed;
        _health = data.health;
        _attackRange = data.attackRange;
        StartCoroutine(AttackRoutine());
    }

    public TowerData Data() {
        return _towerData;
    }
    
    private IEnumerator AttackRoutine() {
        while (true) {
            Attack();
            yield return new WaitForSeconds(_attackSpeed);
        }
    }

    private void OnDrawGizmosSelected() {
        if (_towerData != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,_attackRange);
        }
    }

    protected abstract void Attack();
   
}
