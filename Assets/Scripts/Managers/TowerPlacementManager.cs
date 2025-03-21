using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TowerPlacementManager : MonoBehaviour
{
    //ToDo : UIdaki kulelerin tiklandigi zaman tasinmasini yap,
    //ToDo : sonra enemy'e gec
    
    
    [Inject] private TowerFactory _towerFactory;
    [Inject] private GridManager _gridManager;
    [Inject] private GameManager _gameManager;
    private bool _canPlace = false;
    private bool _isAvailable = false;
    public float activationDistance = 2f;
    public Transform ghostTower;
    public Material ghostMaterial;
    public Camera _camera;
    private Vector3 gridPosition;
    private TowerData selectedTowerData;
    private GameObject _ghostInstance;

    private void Start() {
        _camera.transform.position = new Vector3((_gridManager.maksCellOnRow-1) * _gridManager.cellSize/2f, _camera.transform.position.y, _camera.transform.position.z);
        SetTransparency(true, 0f);
    }
    
    public void SetSelectedTower(TowerData towerData)
    {
        selectedTowerData = towerData;

        if (_ghostInstance != null) {
            Destroy(_ghostInstance);
        }

        _ghostInstance = Instantiate(towerData.towerPrefab);
        ghostTower = _ghostInstance.transform;

        var renderer = _ghostInstance.GetComponentInChildren<Renderer>();
        if (renderer != null) {
            ghostMaterial = renderer.material;
            SetTransparency(true, 0.5f);
        }

    }

    private void Update()
    {
        UpdateGhostTowerPos();
        if (Input.GetMouseButtonDown(0) && _isAvailable) {
            
            //Destroy tower on cell
            if (!_canPlace && selectedTowerData == null) {
                Transform tower = _gridManager.GetCellTransform(gridPosition);
                if(tower == null) return;
                TowerData data = tower.GetComponent<BaseTower>().Data();
                int cost = data.cost;
                _gameManager.AddCurrency(cost);
                Destroy(tower.gameObject);
                _gridManager.SetCellAvailable(gridPosition);
                
            }
            
            //Place tower on cell
            if(_canPlace && selectedTowerData != null) PlaceTower();
            
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
                SetTransparency(_canPlace && selectedTowerData != null, 0.5f);
            }
            else {
                HideGhost();
            }
        }
        else {
            HideGhost();
            _isAvailable = false;
        }
    }
    private void SetTransparency(bool canPlace, float alpha)
    {
        Color color = ghostMaterial.color;
        //if (canPlace) color = Color.green;
        //else color = Color.red;
        color.a = alpha;
        ghostMaterial.color = color;
    }
    private void HideGhost() {
        _canPlace = false;
        SetTransparency(_canPlace, 0f);
        // if (_ghostInstance != null) {
        //     Destroy(_ghostInstance);
        //     _ghostInstance = null;
        //     ghostTower = null;
        // }
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

        BaseTower newTower = _towerFactory.CreateTower(selectedTowerData, position);
        _gridManager.SetCellOccupied(position, newTower.transform);
        selectedTowerData = null;
        HideGhost();
        Debug.Log("Tower has been successfully placed.");
    }
}
