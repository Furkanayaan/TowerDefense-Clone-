using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectsType;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Tower Defense/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public ObjectTypes type;

    [Header("Base Stats")]
    public float moveSpeed = 2f;
    public float maxHealth = 100f;
    public float damageToBase = 5f;
    public int rewardAmount = 10;
    public int damageOnBase = 10;

    [Header("Attack Stats (only for Attacker)")]
    public float attackRange = 2f;
    public float attackSpeed = 1f;
    public float damage = 10f;
    public float projectileSpeed = 5f;
}
