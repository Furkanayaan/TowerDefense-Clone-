using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class TowerPlacementManager : MonoBehaviour
{
    
    [Inject] private TowerFactory _towerFactory;
    [Inject] private GridManager _gridManager;
    [Inject] private GameManager _gameManager;
    [Inject] private GhostTowerPool _ghostTowerPool;
    [Inject] private WaveManager _waveManager;
    private bool _canPlace = false;
    private bool _isAvailable = false;
    public float activationDistance = 2f;
    [ShowInInspector]private Vector3 _gridPosition;
    private TowerData _selectedTowerData;
    private GameObject _ghostInstance;
    private bool _hasAnimated = false;
    private bool _bDeleteTower = false;
    public Transform deleteCube;
    private List<Transform> _allTowers = new();

    private void Start() {
        float cameraXPos = (_gridManager.maksCellOnRow - 1) * _gridManager.cellSize / 2f;
        Camera cam = Camera.main;
        
        cam.transform.position = new Vector3(cameraXPos, cam.transform.position.y, cam.transform.position.z);
        SetTransparency(0.2f);
    }
    
    public void SetSelectedTower(TowerData towerData)
    {
        _selectedTowerData = towerData;

        if (_ghostInstance != null) {
            Destroy(_ghostInstance);
        }

        _ghostInstance = _ghostTowerPool.GetGhost(towerData);
        
        SetTransparency(0.2f);

        _ghostInstance.transform.position = new Vector3(0, 0, 0);

    }

    private void Update()
    {
        if (_waveManager.IsWaveInProgress || _gameManager.IsLose()) {
            _ghostInstance = null;
            return;
        }
        
        UpdateGhostTowerPos();
        if (Input.GetMouseButtonDown(0) && _isAvailable) {
            
            //Destroy tower on cell
            if (!_canPlace && _selectedTowerData == null && _bDeleteTower) {
                Transform tower = _gridManager.GetCellTransform(_gridPosition);
                if(tower == null) return;
                TowerData data = tower.GetComponent<BaseTower>().Data();
                int cost = data.cost;
                _gameManager.AddCurrency(cost);
                _allTowers.Remove(tower);
                Destroy(tower.gameObject);
                _gridManager.SetCellAvailable(_gridPosition);
                RemoveDeleteTowerCube();

            }
            
            //Place tower on cell
            if(_canPlace && _selectedTowerData != null) PlaceTower();
            
        }
    }

    private void UpdateGhostTowerPos() {
        if (_ghostInstance == null) {
            return;
        }
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _gridManager.placementLayer)) {
            _gridPosition = _gridManager.GetCellPosition(hit.point);
            _ghostInstance.transform.position = !_bDeleteTower ? _gridPosition + Vector3.up : _gridPosition;
            
            float distance = Vector3.Distance(hit.point, _gridPosition);

            if (distance <= activationDistance) {
                
                _isAvailable = _gridManager.IsCellAvailable(_gridPosition);
                if(!_isAvailable) return;

                _canPlace = _gridManager.IsCellEmpty(_gridPosition);
                
                SetTransparency(0.7f);
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
        if(_ghostInstance == null || _selectedTowerData == null) return;
        Color color = _selectedTowerData.ghostMaterial.color;
        color.a = alpha;
        _selectedTowerData.ghostMaterial.color = color;
    }
    private void HideGhost() {
        _canPlace = false;
        SetTransparency(0.2f);
        if (_ghostInstance != null) {
            if(_selectedTowerData != null) _ghostTowerPool.ReturnCurrentGhost(_selectedTowerData);
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

        BaseTower newTower = _towerFactory.CreateTower(_selectedTowerData, position);
        _gridManager.SetCellOccupied(position, newTower.transform);
        _allTowers.Add(newTower.transform);
        SetTransparency(1f);
        HideGhost();
        _selectedTowerData = null;
        
        Debug.Log("Tower has been successfully placed.");
    }

    public void DeleteTower() {
        if(_waveManager.IsWaveInProgress || !IsAnyTowerOnCell() || _gameManager.IsLose()) return;
        
        _bDeleteTower = true;
        deleteCube.gameObject.SetActive(true);
        _ghostInstance = deleteCube.gameObject;
    }

    public void RemoveDeleteTowerCube() {
        _bDeleteTower = false;
        deleteCube.gameObject.SetActive(false);
        _ghostInstance = null;
    }

    public bool IsAnyTowerOnCell() {
        return _allTowers.Count > 0;
    }
}
