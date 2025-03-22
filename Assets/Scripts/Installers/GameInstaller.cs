using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    
    public override void InstallBindings()
    {
        Container.Bind<GridManager>().FromComponentInHierarchy().AsSingle();
        
        Container.Bind<TowerFactory>().AsSingle();
        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<TowerPlacementManager>().FromComponentInHierarchy().AsSingle();
        
        Container.Bind<GhostTowerPool>().FromComponentInHierarchy().AsSingle();
        Container.Bind<EnemySpawner>().FromComponentInHierarchy().AsSingle();
        Container.Bind<EnemyRoadPoints>().FromComponentInHierarchy().AsSingle();
        Container.Bind<WaveManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ProjectilePoolManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CurrencyPool>().FromComponentInHierarchy().AsSingle();
        
    }
}
