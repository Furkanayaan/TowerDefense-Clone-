using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class BaseEnemy : MonoBehaviour, IDamageable, IHealth
{
    
    public enum EnemyState
    {
        Idle,
        Moving,
        Attacking,
    }
    private EnemyState _currentState = EnemyState.Idle;
    
    [SerializeField] private EnemyData enemyData;
    protected Transform _targetPoint;
    private NavMeshAgent _navMeshAgent;
    public Action OnDeath;
    public Action OnFinish;
    public float rotationSpeed = 2f;

    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.enabled = false;
        _navMeshAgent.updateRotation = false;
    }

    public void Initialize(Transform target)
    {
        _targetPoint = target;
        MaxHealth = enemyData.maxHealth;
        CurrentHealth = MaxHealth;
        _navMeshAgent.enabled = true;
        _navMeshAgent.speed = enemyData.moveSpeed;
        _navMeshAgent.SetDestination(_targetPoint.position);
        _currentState = EnemyState.Moving;

    }

    private void Update()
    {
        SetStates();
        
    }

    private void SetStates()
    {
        switch (_currentState)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Moving:
                MoveToTarget();
                OnMovingUpdate();
                break;

            case EnemyState.Attacking:
                OnAttackingUpdate();
                break;
            
        }
    }
    protected virtual void OnMovingUpdate() { }
    protected virtual void OnAttackingUpdate() { }

    private void MoveToTarget()
    {
        if (_navMeshAgent.isStopped) _navMeshAgent.isStopped = false;
        RotateToTarget(_targetPoint);
        if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance && !_navMeshAgent.pathPending)
        {
            OnFinish?.Invoke();
            ChangeState(EnemyState.Idle);
        }
    }
    
    
    public void ChangeState(EnemyState newState)
    {
        _currentState = newState;
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public EnemyData GetEnemyData() => enemyData;
    public NavMeshAgent GetEnemyNavMesh => _navMeshAgent; 
    
    
    protected void RotateToTarget(Transform target)
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position);
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    
    }
}