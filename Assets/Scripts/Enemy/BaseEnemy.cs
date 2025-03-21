using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BaseEnemy : MonoBehaviour {
    
    [SerializeField] protected EnemyData enemyData;
    private Transform _targetPoint;
    private float _currentHealth;
    private bool _isMoving = true;
    private bool _isFinishing = false;
    protected Rigidbody _rb;

    public Action OnDeathOrFinish;

    public void Initialize(Transform target)
    {
        _targetPoint = target;
        _currentHealth = enemyData.maxHealth;
        _isMoving = true;
        _isFinishing = false;
        _rb = GetComponent<Rigidbody>();
    }

    protected virtual void Update() {
        if(!_isMoving || _targetPoint == null || _isFinishing) return;
        MoveToTarget();
    }

    private void MoveToTarget() {
        Vector3 dir = (_targetPoint.position - transform.position).normalized;
        Vector3 movement = dir * enemyData.moveSpeed;

        _rb.velocity = movement;

        if (Vector3.Distance(transform.position, _targetPoint.position) < 2f) {
            OnDeathOrFinish?.Invoke();
            _isFinishing = true;
        }
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        if (_currentHealth <= 0) {
            OnDeathOrFinish?.Invoke();
        }
    }

    public EnemyData GetEnemyData() => enemyData;
    public void StopMovement() {
        _isMoving = false;
        if (_rb != null) _rb.velocity = Vector3.zero;
    }

    public void ResumeMovement() => _isMoving = true;
}
