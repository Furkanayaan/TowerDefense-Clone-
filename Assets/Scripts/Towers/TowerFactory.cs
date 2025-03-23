using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TowerFactory
{
    [Inject] private DiContainer _container;

    public BaseTower CreateTower(TowerData towerData, Vector3 position)
    {
        // Instantiate the tower prefab using Zenject and inject dependencies
        BaseTower newTower = _container.InstantiatePrefabForComponent<BaseTower>(towerData.towerPrefab, position, Quaternion.identity, null);
        // Initialize tower with its corresponding data
        newTower.Initialize(towerData);
        
        // Get the tower's visual height to adjust its Y position
        Vector3 towerScale = newTower.transform.GetChild(0).localScale;
        Vector3 towerPos = newTower.transform.position;
        
        // Reposition the tower so it sits correctly on the ground
        Vector3 towerNewPos = new Vector3(towerPos.x, towerScale.y / 2f, towerPos.z);
        newTower.transform.position = towerNewPos;
        
        return newTower;
    } 
}
