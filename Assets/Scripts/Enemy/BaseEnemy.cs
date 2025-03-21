using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BaseEnemy : MonoBehaviour, IDamageable, IHealth {

    
    [SerializeField] private EnemyData enemyData;
    protected Transform _targetPoint;
    private bool _isMoving = true;
    private bool _isFinishing = false;
    protected Rigidbody _rb;
    public Action OnDeath;
    public Action OnFinish;
    public float rotationSpeed = 2f;
    
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }

    public void Initialize(Transform target)
    {
        _targetPoint = target;
        MaxHealth = enemyData.maxHealth;
        CurrentHealth = MaxHealth;
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
        
        RotateToTarget(_targetPoint);
        _rb.velocity = movement;

        if (Vector3.Distance(transform.position, _targetPoint.position) < 2f) {
            OnFinish?.Invoke();
            _isFinishing = true;
        }
    }

    

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0) {
            OnDeath?.Invoke();
        }
    }

    public EnemyData GetEnemyData() => enemyData;
    public void StopMovement() {
        _isMoving = false;
        if (_rb != null) _rb.velocity = Vector3.zero;
    }

    public void ResumeMovement() => _isMoving = true;
    
    protected void RotateToTarget(Transform targetPosition) {
        if (targetPosition == null) return;

        Vector3 direction = (targetPosition.position - transform.position).normalized;
        direction.y = 0f; 

        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(-direction);
        Quaternion smoothRotation = Quaternion.Slerp(_rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        _rb.MoveRotation(smoothRotation);
    }
}
