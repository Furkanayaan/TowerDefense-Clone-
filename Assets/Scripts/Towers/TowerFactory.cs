using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TowerFactory
{
    [Inject] private DiContainer _container;

    public BaseTower CreateTower(TowerData towerData, Vector3 position)
    {
        BaseTower newTower = _container.InstantiatePrefabForComponent<BaseTower>(towerData.towerPrefab, position, Quaternion.identity, null);
        newTower.Initialize(towerData);
        Vector3 towerScale = newTower.transform.GetChild(0).localScale;
        Vector3 towerPos = newTower.transform.position;
        
        Vector3 towerNewPos = new Vector3(towerPos.x, towerScale.y / 2f, towerPos.z);
        
        newTower.transform.position = towerNewPos;
        return newTower;
    } 
}
