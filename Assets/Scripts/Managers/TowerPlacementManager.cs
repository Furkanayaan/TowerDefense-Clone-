using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class TowerPlacementManager : MonoBehaviour
{
    //ToDo : UIdaki kulelerin tiklandigi zaman tasinmasini yap,
    //ToDo : sonra enemy'e gec
    
    
    [Inject] private TowerFactory _towerFactory;
    [Inject] private GridManager _gridManager;
    [Inject] private GameManager _gameManager;
    [Inject] private GhostTowerPool _ghostTowerPool;
    private bool _canPlace = false;
    private bool _isAvailable = false;
    public float activationDistance = 2f;
    public Camera _camera;
    [ShowInInspector]private Vector3 gridPosition;
    private TowerData selectedTowerData;
    private GameObject _ghostInstance;
    private bool _hasAnimated = false;
    private bool _bDeleteTower = false;
    public Transform deleteCube;

    private void Start() {
        _camera.transform.position = new Vector3((_gridManager.maksCellOnRow-1) * _gridManager.cellSize/2f, _camera.transform.position.y, _camera.transform.position.z);
        SetTransparency(0f);
    }
    
    public void SetSelectedTower(TowerData towerData)
    {
        selectedTowerData = towerData;

        if (_ghostInstance != null) {
            Destroy(_ghostInstance);
        }

        _ghostInstance = _ghostTowerPool.GetGhost(towerData);
        
        SetTransparency(0.2f);

        _ghostInstance.transform.position = new Vector3(0, 0, 0);

    }

    private void Update()
    {
        UpdateGhostTowerPos();
        if (Input.GetMouseButtonDown(0) && _isAvailable) {
            
            //Destroy tower on cell
            if (!_canPlace && selectedTowerData == null && _bDeleteTower) {
                Transform tower = _gridManager.GetCellTransform(gridPosition);
                if(tower == null) return;
                TowerData data = tower.GetComponent<BaseTower>().Data();
                int cost = data.cost;
                _gameManager.AddCurrency(cost);
                Destroy(tower.gameObject);
                _gridManager.SetCellAvailable(gridPosition);
                RemoveDeleteTowerCube();

            }
            
            //Place tower on cell
            if(_canPlace && selectedTowerData != null) PlaceTower();
            
        }
    }

    private void UpdateGhostTowerPos() {
        if (_ghostInstance == null) {
            return;
        }
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _gridManager.placementLayer)) {
            gridPosition = _gridManager.GetCellPosition(hit.point);
            _ghostInstance.transform.position = !_bDeleteTower ? gridPosition + Vector3.up : gridPosition;
            
            float distance = Vector3.Distance(hit.point, gridPosition);

            if (distance <= activationDistance) {
                
                _isAvailable = _gridManager.IsCellAvailable(gridPosition);
                if(!_isAvailable) return;

                _canPlace = _gridManager.IsCellEmpty(gridPosition);
                
                SetTransparency(0.5f);
                if (!_hasAnimated && !_bDeleteTower) {
                    AnimateGhost();
                    _hasAnimated = true;
                }
            }
            else {
                _hasAnimated = false;
                SetTransparency(0.2f);
            }
        }
        else if (_ghostInstance != null) {
            _isAvailable = false;
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float enter)) {
                Vector3 worldPos = ray.GetPoint(enter);
                _ghostInstance.transform.position = Vector3.Lerp(_ghostInstance.transform.position, worldPos + Vector3.up , 10f*Time.deltaTime);
                SetTransparency(0.2f);
            }

            _hasAnimated = false;
        }
    }
    
    private void AnimateGhost()
    {
        if (_ghostInstance == null) return;

        _ghostInstance.transform.localScale = Vector3.one * 0.8f;
        _ghostInstance.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
    private void SetTransparency(float alpha)
    {
        if(_ghostInstance == null || selectedTowerData == null) return;
        Color color = selectedTowerData.ghostMaterial.color;
        color.a = alpha;
        selectedTowerData.ghostMaterial.color = color;
    }
    private void HideGhost() {
        _canPlace = false;
        SetTransparency(0f);
        if (_ghostInstance != null) {
            if(selectedTowerData != null) _ghostTowerPool.ReturnCurrentGhost(selectedTowerData);
            _ghostInstance = null;
        }
    }

    private void PlaceTower()
    {
        Vector3 position = _ghostInstance.transform.position;

        Debug.Log($"Placement Point: {position}");
        if (!_gridManager.IsCellAvailable(position))
        {
            Debug.Log("ERROR: The grid considers this cell as filled!");
            return;
        }

        BaseTower newTower = _towerFactory.CreateTower(selectedTowerData, position);
        _gridManager.SetCellOccupied(position, newTower.transform);
        SetTransparency(1f);
        HideGhost();
        selectedTowerData = null;
        
        Debug.Log("Tower has been successfully placed.");
    }

    public void DeleteTower() {
        _bDeleteTower = true;
        deleteCube.gameObject.SetActive(true);
        _ghostInstance = deleteCube.gameObject;
    }

    public void RemoveDeleteTowerCube() {
        _bDeleteTower = false;
        deleteCube.gameObject.SetActive(false);
        _ghostInstance = null;
    }
}
