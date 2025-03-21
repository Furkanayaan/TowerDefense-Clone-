using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTower", menuName = "Tower Defense/Tower Data")]
public class TowerData : ScriptableObject {
    public string towerName;
    public int cost;
    public float damage;
    public float attackSpeed;
    public float health;
    public float attackRange;
    public GameObject towerPrefab;
}
