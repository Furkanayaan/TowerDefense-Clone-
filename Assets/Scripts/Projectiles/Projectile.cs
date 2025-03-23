using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private IDamageable _targetDamageable; // Reference to the target's damageable interface
    private Transform _targetTransform; // Transform of the target
    private float _speed;
    private float _damage; // Damage dealt on hit

    private ProjectilePoolManager _pool; // Reference to the projectile pool manager

    public void Initialize(IDamageable targetDamageable, Transform targetTransform, float moveSpeed, float damageAmount, ProjectilePoolManager pool)
    {
        _targetDamageable = targetDamageable;
        _targetTransform = targetTransform; 
        _speed = moveSpeed;
        _damage = damageAmount;
        _pool = pool;
    }

    private void Update()
    {
        // If target is null or inactive, return to pool
        if (_targetTransform == null || !_targetTransform.gameObject.activeInHierarchy)
        {
            _pool.ReturnToPool(gameObject);
            return;
        }
        
        // Move the projectile towards the target
        transform.position = Vector3.MoveTowards(transform.position, _targetTransform.position, _speed * Time.deltaTime);

        // If close enough to target, deal damage and return to pool
        if ((transform.position - _targetTransform.position).sqrMagnitude < 0.04f)
        {
            // Apply damage to the target
            _targetDamageable.TakeDamage(_damage);
            // Return projectile to the pool
            _pool.ReturnToPool(gameObject);
        }
    }
}
