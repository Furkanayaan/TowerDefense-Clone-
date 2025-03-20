using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    
    public float cellSize = 2f;
    public GameObject gridVisualPrefab;
    public LayerMask placementLayer;
    public int rowCount = 0;
    public List<int> cellCount = new List<int>();
    public int maksCellOnRow = 3;
    [ShowInInspector]private Dictionary<Vector2Int, bool> gridCells = new();
    [ShowInInspector]private Dictionary<Vector2Int, Transform> gridTower = new();

    private void Start()
    {
        cellCount.Add(1);
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        for (int y = 0; y <= rowCount; y++)
        {
            for (int x = 0; x < cellCount[y]; x++)
            {
                Vector2Int cell = new(x, y);
                if (!gridCells.ContainsKey(cell) && !gridTower.ContainsKey(cell)) {
                    gridCells[cell] = false;
                    gridTower[cell] = null;
                    //Draw grid
                    Vector3 position = new Vector3(x * cellSize, 0f, y * cellSize);
                    Instantiate(gridVisualPrefab, position, Quaternion.identity);
                }
            }
        }
        Debug.Log("Grid has been created and all cells are empty.");
    }

    public Vector3 GetCellPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / cellSize);
        int y = Mathf.RoundToInt(worldPosition.z / cellSize);
        Vector3 snappedPosition = new Vector3(x * cellSize, 0, y * cellSize);

        Debug.Log($"GridManager GetCellPosition: World Position {worldPosition}, Grid Position {snappedPosition}");
        return snappedPosition;
    }

    public bool IsCellAvailable(Vector3 worldPosition)
    {
        Vector2Int cell = WorldToGrid(worldPosition);
        if (!gridCells.ContainsKey(cell))
        {
            Debug.Log($"ERROR: Cell {cell} could not be found in the grid!");
            return false;
        }

        Debug.Log($"Cell {cell} status: {(gridCells[cell] ? "FILLED" : "EMPTY")}");
        return true;
    }

    public bool IsCellEmpty(Vector3 worldPosition) {
        Vector2Int cell = WorldToGrid(worldPosition);
        return !gridCells[cell];
    }

    public void SetCellOccupied(Vector3 worldPosition, Transform tower)
    {
        Vector2Int cell = WorldToGrid(worldPosition);
        if (gridCells.ContainsKey(cell))
        {
            gridCells[cell] = true;
            gridTower[cell] = tower;
            Debug.Log($"Cell {cell} is now marked as filled.");
        }
    }

    public void SetCellAvailable(Vector3 worldPosition)
    {
        Vector2Int cell = WorldToGrid(worldPosition);
        Debug.Log("FullyCellPos" + cell);
        
        if (gridCells.ContainsKey(cell) && gridTower[cell] != null) {
            gridCells[cell] = false;
            Destroy(gridTower[cell].gameObject);
            gridTower[cell] = null;
            Debug.Log($"Cell {cell} has been marked as empty again.");
        }
    }

    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / cellSize);
        int y = Mathf.RoundToInt(worldPosition.z / cellSize);
        return new Vector2Int(x, y);
    }
}
