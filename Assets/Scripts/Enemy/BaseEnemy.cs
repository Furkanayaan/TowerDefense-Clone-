using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class BaseEnemy : MonoBehaviour, IDamageable
{
    // Enemy AI states
    public enum EnemyState
    {
        Idle,
        Moving,
        Attacking,
    }
    private EnemyState _currentState = EnemyState.Idle;
    
    [SerializeField] private EnemyData enemyData;
    // Target destination (usually base)
    private Transform _targetPoint;
    private NavMeshAgent _navMeshAgent;
    // State machine delegate map
    private Dictionary<EnemyState, Action> _stateBehaviors;
    
    // Event callbacks
    public Action OnDeath;
    public Action OnFinish;
    public float rotationSpeed = 2f;

    // Health properties
    public float MaxHealth { get; set; } 
    public float CurrentHealth { get; set; }

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.enabled = false;
        _navMeshAgent.updateRotation = false;
        
        // Define state behaviors for the FSM
        _stateBehaviors = new Dictionary<EnemyState, Action>
        {
            { EnemyState.Idle, () => { } },
            {
                EnemyState.Moving, () =>
                {
                    MoveToTarget();
                    OnMovingUpdate();
                }
            },
            { EnemyState.Attacking, () =>
            {
                OnAttackingUpdate();
            } 
            }
        };
    }

    // Called externally when enemy is spawned
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
        // Run the current state's behavior each frame
        _stateBehaviors[_currentState]?.Invoke();
    }
    
    // To be overridden by child classes
    protected virtual void OnMovingUpdate() { }
    protected virtual void OnAttackingUpdate() { }

    // Handles movement and path completion
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
    
    
    // Changes the current state
    public void ChangeState(EnemyState newState)
    {
        _currentState = newState;
    }

    // Damageable interface implementation
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
    
    
    // Smooth rotation towards a target
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