using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private IDamageable _targetDamageable;
    private Transform _targetTransform;
    private float _speed;
    private float _damage;

    private ProjectilePoolManager _pool;

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
        if (_targetTransform == null)
        {
            _pool.ReturnProjectile(gameObject);
            return;
        }
        
        Vector3 dir = (_targetTransform.transform.position - transform.position).normalized;
        transform.position += dir * _speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, _targetTransform.transform.position) < 0.2f)
        {
            _targetDamageable.TakeDamage(_damage);
            _pool.ReturnProjectile(gameObject);
        }
    }
}
