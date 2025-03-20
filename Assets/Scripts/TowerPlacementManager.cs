using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TowerPlacementManager : MonoBehaviour
{
    [Inject] private Tower.Factory _towerFactory;
    [Inject] private GridManager _gridManager;
    private bool _canPlace = false;
    private bool _isAvailable = false;
    public float activationDistance = 2f;
    public Transform ghostTower;
    public Material ghostMaterial;
    public Camera _camera;
    private Vector3 gridPosition;

    private void Start() {
        _camera.transform.position = new Vector3((_gridManager.maksCellOnRow-1) * _gridManager.cellSize/2f, _camera.transform.position.y, _camera.transform.position.z);
        SetTransparency(true, 0f);
    }

    private void Update()
    {
        UpdateGhostTowerPos();
        if (Input.GetMouseButtonDown(0) && _isAvailable) {
            //Place tower on cell
            if(_canPlace) PlaceTower();
            //Destroy tower on cell
            else _gridManager.SetCellAvailable(gridPosition);
            
        }
    }

    private void UpdateGhostTowerPos() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _gridManager.placementLayer)) {
            gridPosition = _gridManager.GetCellPosition(hit.point);
            float distance = Vector3.Distance(hit.point, gridPosition);

            if (distance <= activationDistance) {
                _isAvailable = _gridManager.IsCellAvailable(gridPosition);
                if(!_isAvailable) return;

                _canPlace = _gridManager.IsCellEmpty(gridPosition);
                ghostTower.position = gridPosition;
                SetTransparency(_canPlace, 0.5f);
            }
            else {
                HideGhost();
            }
        }
        else {
            HideGhost();
            _isAvailable = false;
            _canPlace = false;
        }
    }
    private void SetTransparency(bool canPlace, float alpha)
    {
        Color color = ghostMaterial.color;
        if (canPlace) color = Color.green;
        else color = Color.red;
        color.a = alpha;
        ghostMaterial.color = color;
    }
    private void HideGhost() {
        _canPlace = false;
        SetTransparency(_canPlace, 0f);
    }

    private void PlaceTower()
    {
        Vector3 position = ghostTower.position;

        Debug.Log($"Placement Point: {position}");
        if (!_gridManager.IsCellAvailable(position))
        {
            Debug.Log("ERROR: The grid considers this cell as filled!");
            return;
        }

        Tower newTower = _towerFactory.Create();
        newTower.transform.position = new Vector3(position.x,newTower.transform.localScale.y/2f,position.z);
        _gridManager.SetCellOccupied(position, newTower.transform);
        Debug.Log("Tower has been successfully placed.");
    }
}
