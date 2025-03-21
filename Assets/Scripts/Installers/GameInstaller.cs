using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public GridManager gridManagerPrefab; // GridManager prefab

    public override void InstallBindings()
    {
        
        // Grid Manager
        GridManager gridManagerInstance = 
            Container.InstantiatePrefabForComponent<GridManager>(gridManagerPrefab);
        Container.Bind<GridManager>().FromInstance(gridManagerInstance).AsSingle();
        
        Container.Bind<TowerFactory>().AsSingle();
        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<TowerPlacementManager>().FromComponentInHierarchy().AsSingle();
        
        Container.Bind<GhostTowerPool>().FromComponentInHierarchy().AsSingle();
    }
}
