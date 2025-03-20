using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public GameObject towerPrefab; // Tower prefab
    public GridManager gridManagerPrefab; // GridManager prefab

    public override void InstallBindings()
    {
        // Tower Factory
        Container.BindFactory<Tower, Tower.Factory>()
            .FromComponentInNewPrefab(towerPrefab)
            .AsTransient();

        // Grid Manager
        GridManager gridManagerInstance = 
            Container.InstantiatePrefabForComponent<GridManager>(gridManagerPrefab);
        Container.Bind<GridManager>().FromInstance(gridManagerInstance).AsSingle();
    }
}
