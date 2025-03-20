using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GhostTower : MonoBehaviour
{
    [Inject] private GridManager _gridManager;
    public Material ghostMaterial;
    private Renderer _renderer;
    private bool _canPlace = false;
    private bool _isAvailable = false;
    private Vector3 _currentGridPosition;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        SetTransparency(true);
    }

    private void Update()
    {
        FollowMouse();
    }

    private void FollowMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        // If placementLayer is not specified, scan all layers
        int layerMask = _gridManager.placementLayer.value == 0 ? ~0 : _gridManager.placementLayer;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            _currentGridPosition = _gridManager.GetCellPosition(hit.point);
            _isAvailable = _gridManager.IsCellAvailable(_currentGridPosition);
            if(!_isAvailable) return;
            
            
            _canPlace = _gridManager.IsCellEmpty(_currentGridPosition);
            transform.position = _currentGridPosition;
            
            Debug.Log($"GhostTower New Position: {_currentGridPosition}, Placement Status: {_canPlace}");

            SetTransparency(_canPlace);
        }
    }

    private void SetTransparency(bool canPlace)
    {
        Color color = ghostMaterial.color;
        if (canPlace) color = Color.green;
        else color = Color.red;
        color.a = 0.5f;
        ghostMaterial.color = color;
    }

    public Vector3 GetGhostPosition() => _currentGridPosition;
    public bool CanPlace() => _canPlace;
}
