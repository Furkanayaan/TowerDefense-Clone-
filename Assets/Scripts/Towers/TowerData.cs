using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectsType;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewTower", menuName = "Tower Defense/Tower Data")]
public class TowerData : ScriptableObject {
    [FormerlySerializedAs("towerType")] public ObjectTypes objectType;
    public int cost;
    public float damage;
    public float attackSpeed;
    public float health;
    public float attackRange;
    public GameObject towerPrefab;
    public Material ghostMaterial;
}
