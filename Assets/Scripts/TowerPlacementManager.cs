using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TowerPlacementManager : MonoBehaviour
{
    [Inject] private Tower.Factory _towerFactory;
    [Inject] private GridManager _gridManager;
    [Inject] private GhostTower _ghostTower;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 ghostPosition = _ghostTower.GetGhostPosition();
            Debug.Log($"GhostTower Position: {ghostPosition}");

            if (_ghostTower.CanPlace())
            {
                Debug.Log("Placing tower...");
                PlaceTower();
            }
            else
            {
                Debug.Log("A tower cannot be placed on this cell!");
            }
        }
    }

    private void PlaceTower()
    {
        Vector3 position = _ghostTower.GetGhostPosition();

        Debug.Log($"Placement Point: {position}");
        if (!_gridManager.IsCellAvailable(position))
        {
            Debug.Log("ERROR: The grid considers this cell as filled!");
            return;
        }

        Tower newTower = _towerFactory.Create();
        newTower.transform.position = position;
        _gridManager.SetCellOccupied(position);
        Debug.Log("Tower has been successfully placed.");
    }
}
