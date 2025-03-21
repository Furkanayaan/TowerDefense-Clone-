using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [SerializeField] protected EnemyData enemyData;
    private Transform targetPoint;
    private float _currentHealth;
    private bool isMoving = true;
    protected Rigidbody _rb;

    public System.Action OnDeath;

    public void Initialize(Transform target)
    {
        targetPoint = target;
        _currentHealth = enemyData.maxHealth;
        isMoving = true;
        _rb = GetComponent<Rigidbody>();
    }

    protected virtual void Update() {
        if(!isMoving || targetPoint == null) return;
        MoveToTarget();
    }

    private void MoveToTarget() {
        Vector3 dir = (targetPoint.position - transform.position).normalized;
        Vector3 movement = dir * enemyData.moveSpeed;

        _rb.velocity = movement;

        if (Vector3.Distance(transform.position, targetPoint.position) < 2f) {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    public EnemyData GetEnemyData() => enemyData;
    public void StopMovement() {
        isMoving = false;
        if (_rb != null) _rb.velocity = Vector3.zero;
    }

    public void ResumeMovement() => isMoving = true;
}
