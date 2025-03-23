using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GridManager : MonoBehaviour
{
    [Inject] private GameManager _gameManager;
    [Range(1,5)]
    public float cellSize = 2f; // Size of each grid cell
    public GameObject gridVisualPrefab; // Prefab to visually represent a grid cell
    public LayerMask placementLayer; // Layer used for placement raycasts
    private int _rowCount = 0; // Current number of rows
    private List<int> _cellCount = new List<int>(); // List holding cell count per row
    [Range(1,5)]
    public int maksCellOnRow = 3; // Max cells allowed in a row
    private Dictionary<Vector2Int, bool> _gridCells = new(); // Dictionary to track if a cell is occupied
    private Dictionary<Vector2Int, Transform> _gridTower = new(); // Dictionary to hold tower references
    //Only checks the interactable state of the buttons.
    public Action OnChangedCell;
    public int cellCost;

    private void Start()
    {
        // Start with one cell in the first row
        _cellCount.Add(1);
        OnChangedCell?.Invoke();
        // Generate initial grid
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        // Loop through all defined rows and generate corresponding cells
        for (int y = 0; y <= _rowCount; y++)
        {
            for (int x = 0; x < _cellCount[y]; x++)
            {
                Vector2Int cell = new(x, y);
                if (!_gridCells.ContainsKey(cell) && !_gridTower.ContainsKey(cell))
                {
                    // Mark cell as available
                    _gridCells[cell] = false; 
                    // No tower on this cell yet
                    _gridTower[cell] = null;
                    // Calculate world position
                    Vector3 position = new Vector3(x * cellSize, 0f, y * cellSize);
                    // Instantiate visual grid cell
                    Instantiate(gridVisualPrefab, position, Quaternion.identity);
                }
            }
        }
        OnChangedCell?.Invoke();
    }

    public Vector3 GetCellPosition(Vector3 worldPosition)
    {
        // Snap world position to nearest grid position
        int x = Mathf.RoundToInt(worldPosition.x / cellSize);
        int y = Mathf.RoundToInt(worldPosition.z / cellSize);
        Vector3 snappedPosition = new Vector3(x * cellSize, 0, y * cellSize);

        return snappedPosition;
    }

    public bool IsCellValid(Vector3 worldPosition)
    {
        // Check if the cell exists in the grid
        Vector2Int cell = WorldToGrid(worldPosition);
        if (!_gridCells.ContainsKey(cell))
        {
            return false;
        }

        return true;
    }

    public bool IsCellEmpty(Vector3 worldPosition)
    {
        // Check if the cell is unoccupied
        Vector2Int cell = WorldToGrid(worldPosition);
        return !_gridCells[cell];
    }

    public void SetCellOccupied(Vector3 worldPosition, Transform tower)
    {
        // Mark a cell as occupied and assign a tower to it
        Vector2Int cell = WorldToGrid(worldPosition);
        if (_gridCells.ContainsKey(cell))
        {
            _gridCells[cell] = true;
            _gridTower[cell] = tower;
        }
    }

    public void SetCellAvailable(Transform tower)
    {
        // Mark a cell as available again by removing the tower reference
        Vector2Int cell = WorldToGrid(tower.position);
        if (_gridCells.ContainsKey(cell) && _gridTower[cell] != null)
        {
            _gridCells[cell] = false;
            _gridTower[cell] = null;
        }
    }

    public Transform GetTransformFromCell(Vector3 worldPosition)
    {
        // Return the tower transform at the specified grid cell
        Vector2Int cell = WorldToGrid(worldPosition);
        if (!_gridTower.ContainsKey(cell)) return null;
        return _gridTower[cell];
    }

    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        // Convert world position to grid coordinates
        int x = Mathf.RoundToInt(worldPosition.x / cellSize);
        int y = Mathf.RoundToInt(worldPosition.z / cellSize);
        return new Vector2Int(x, y);
    }

    // Return all cell on scene
    public int ReturnAllCellCount()
    {
        if (_cellCount.Count < 1) return 0;
        int count = 0;
        for (int i = 0; i < _cellCount.Count; i++)
        {
            count += _cellCount[i];
        }

        return count;
    }
    
    // Called when the player clicks the Add Cell button
    public void AddCell()
    {
        if (_gameManager.TrySpendCurrency(cellCost))
        {
            _cellCount[_rowCount]++;
            if (_cellCount[_rowCount] >= maksCellOnRow)
            {
                _rowCount++;
                _cellCount.Add(0);
            }

            GenerateGrid();
        }
    }
}