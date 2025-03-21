using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerEnemy : BaseEnemy {
    private BaseTower _targetTower;
    
    private float _attackRange => enemyData.attackRange;
    public float rotationSpeed = 5f;
    protected override void Update()
    {
        if (_targetTower != null)
        {
            float dist = Vector3.Distance(transform.position, _targetTower.transform.position);
            if (dist > _attackRange || _targetTower == null) {
                _targetTower = null;
                ResumeMovement();
            }
            else {
                RotateToTarget();
                StopMovement();
                return;
            }
        }
        else {
            SearchForTower();
        }

        base.Update();
    }

    private void SearchForTower()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _attackRange);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out BaseTower tower))
            {
                _targetTower = tower;
                StopMovement();
                break;
            }
        }
    }
    
    private void RotateToTarget()
    {
        if (_targetTower == null) return;

        Vector3 direction = (_targetTower.transform.position - transform.position).normalized;
        direction.y = 0f; 

        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(-direction);
        Quaternion smoothRotation = Quaternion.Slerp(_rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        _rb.MoveRotation(smoothRotation);
    }
}
