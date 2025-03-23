using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class TowerPlacementManager : MonoBehaviour {
    [Inject] private TowerFactory _towerFactory;
    [Inject] private GridManager _gridManager;
    [Inject] private GameManager _gameManager;
    [Inject] private GhostTowerPool _ghostTowerPool;
    [Inject] private WaveManager _waveManager;

    // Enum to manage player interaction states
    private enum InteractionState { None, Placing, Deleting }

    // Current interaction state
    [ShowInInspector]private InteractionState _currentState = InteractionState.None;

    public float activationDistance = 2f; // Distance to activate cell interaction
    public Transform deleteCube; // Reference to delete cube

    private Vector3 _gridPosition; // Current grid cell position
    private GameObject _ghostInstance; // Current ghost tower instance
    private TowerData _selectedTowerData; // Currently selected tower data
    private bool _hasAnimated = false; // Animation flag
    private List<Transform> _allTowers = new(); // List of all placed towers
    private Camera _camera;
    public bool isDeleting; // Is in delete mode

    public Action<int> OnTowerCountChanged;
    //Only checks the interactable state of the buttons.
    public Action OnSelectedDataChanged;
    //Only checks the interactable state of the buttons.
    public Action OnIsDeletingChanged;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start() {
        float cameraXPos = (_gridManager.maksCellOnRow - 1) * _gridManager.cellSize / 2f;
        SetXPosition(_camera.transform, cameraXPos);
        OnTowerCountChanged?.Invoke(_allTowers.Count);
        OnSelectedDataChanged?.Invoke();
        OnIsDeletingChanged?.Invoke();
    }
    
    // Set camera X position based on grid width and cell size
    private void SetXPosition(Transform t, float x)
    {
        Vector3 pos = t.position;
        pos.x = x;
        t.position = pos;
    }

    // Update loop handling ghost updates and click input
    private void Update() {
        if (_waveManager.IsWaveInProgress || _gameManager.IsLose()) {
            if (_ghostInstance != null) {
                if(_selectedTowerData != null) _gameManager.GoldPoolToGo(_selectedTowerData.cost, _ghostInstance.transform.position);
                HideGhost();
            }
            return;
        }
        UpdateGhostTowerPosition();
        MouseButtonDown();
        
    }
    
    // Handles input click for placing or deleting towers
    private void MouseButtonDown()
    {
        if (Input.GetMouseButtonDown(0)) {
            if (_currentState == InteractionState.Deleting) TryDeleteTower();
            else if (_currentState == InteractionState.Placing) TryPlaceTower();
        }
    }

    // Assigns selected tower and prepares ghost tower
    public void SetSelectedTower(TowerData towerData) {
        _selectedTowerData = towerData;
        OnSelectedDataChanged?.Invoke();
        _currentState = InteractionState.None;
        _ghostInstance = _ghostTowerPool.GetGhost(towerData);
        SetGhostTransparency(0.2f);
        _ghostInstance.transform.position = Vector3.zero;
    }

    // Handles position update of ghost tower based on mouse position
    private void UpdateGhostTowerPosition() {
        if (_ghostInstance == null) return;

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _gridManager.placementLayer)) {
            _gridPosition = _gridManager.GetCellPosition(hit.point);
            _ghostInstance.transform.position = !isDeleting ? _gridPosition + Vector3.up : _gridPosition;
            
            float distance = Vector3.Distance(hit.point, _gridPosition);

            if (distance <= activationDistance)
            {
                bool isCellValid = _gridManager.IsCellValid(_gridPosition);
                if(!isCellValid) return;
                bool isCellEmpty = _gridManager.IsCellEmpty(_gridPosition);
                // If is cell empty and state is not delete mode
                if (isCellEmpty && !isDeleting) {
                    _currentState = InteractionState.Placing;
                    SetGhostTransparency(0.7f);
                    if (!_hasAnimated) {
                        AnimateGhost();
                        _hasAnimated = true;
                    }
                }
                else
                {
                    _currentState = InteractionState.Deleting;
                }
            }
            else ResetGhostVisual();
        } else
        {
            _currentState = InteractionState.None;
            ResetGhostVisual();
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float enter)) {
                Vector3 worldPos = ray.GetPoint(enter);
                _ghostInstance.transform.position = Vector3.Lerp(_ghostInstance.transform.position, worldPos + Vector3.up, 10f * Time.deltaTime);
            }
        }
    }

    // Reset ghost visual to default
    private void ResetGhostVisual() {
        _hasAnimated = false;
        SetGhostTransparency(0.2f);
    }

    // Set ghost material transparency
    private void SetGhostTransparency(float alpha) {
        if (_ghostInstance == null || _selectedTowerData == null) return;
        Color color = _selectedTowerData.ghostMaterial.color;
        color.a = alpha;
        _selectedTowerData.ghostMaterial.color = color;
    }

    // Animate ghost tower on cell entry
    private void AnimateGhost() {
        _ghostInstance.transform.localScale = Vector3.one * 0.8f;
        _ghostInstance.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    // Place tower on grid and finalize
    private void TryPlaceTower() {
        Vector3 position = _gridPosition + Vector3.up;
        BaseTower newTower = _towerFactory.CreateTower(_selectedTowerData, position);
        _gridManager.SetCellOccupied(position, newTower.transform);
        
        _allTowers.Add(newTower.transform);
        OnTowerCountChanged?.Invoke(_allTowers.Count);
        HideGhost();
        OnSelectedDataChanged?.Invoke();
        _currentState = InteractionState.None;
    }

    // Enable delete mode and assign delete cube
    public void DeleteTower() {
        //if (_waveManager.IsWaveInProgress || !IsAnyTowerOnCell() || _gameManager.IsLose() || _selectedTowerData != null) return;
        isDeleting = true;
        OnIsDeletingChanged?.Invoke();
        deleteCube.gameObject.SetActive(true);
        _ghostInstance = deleteCube.gameObject;
    }

    // Attempt to delete tower on selected cell
    private void TryDeleteTower() {
        Transform tower = _gridManager.GetTransformFromCell(_gridPosition);
        if (tower == null) return;
        TowerData data = tower.GetComponent<BaseTower>().Data();
        _gameManager.GoldPoolToGo(data.cost, tower.position);
        RemoveTowerFromList(tower);
        Destroy(tower.gameObject);
        RemoveDeleteTowerCube();
    }

    // Hide and reset delete cube
    private void RemoveDeleteTowerCube() {
        _currentState = InteractionState.None;
        deleteCube.gameObject.SetActive(false);
        isDeleting = false;
        _ghostInstance = null;
        OnIsDeletingChanged?.Invoke();
    }

    // Hide ghost tower and return to pool
    private void HideGhost() {
        SetGhostTransparency(0.2f);
        if (_ghostInstance != null && _selectedTowerData != null) {
            _ghostTowerPool.ReturnCurrentGhost();
        }

        if (isDeleting)
        {
            RemoveDeleteTowerCube();
        }
        OnIsDeletingChanged?.Invoke();
        _selectedTowerData = null;
        _ghostInstance = null;
    }

    public bool IsAnyTowerOnCell() => _allTowers.Count > 0;
    public int TotalTower() => _allTowers.Count;

    public TowerData GetSelectedData()
    {
        return _selectedTowerData;
    }

    // Remove tower from grid and internal list
    public void RemoveTowerFromList(Transform tower) {
        _allTowers.Remove(tower);
        OnTowerCountChanged?.Invoke(_allTowers.Count);
        _gridManager.SetCellAvailable(tower);
    }

    // Check if all grid cells are filled with towers
    public bool AreAllCellsFully()
    {
        return _gridManager.ReturnAllCellCount() == _allTowers.Count;
    }
}
