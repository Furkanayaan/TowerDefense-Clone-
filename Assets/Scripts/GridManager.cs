using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 10;  
    public int gridHeight = 10; 
    public float cellSize = 2f;
    public GameObject gridVisualPrefab;
    public LayerMask placementLayer; 

    private Dictionary<Vector2Int, bool> gridCells = new();

    private void Start()
    {
        GenerateGrid();
        DrawGrid();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2Int cell = new(x, y);
                gridCells[cell] = false;
            }
        }
        Debug.Log("Grid has been created and all cells are empty.");
    }

    private void DrawGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = new Vector3(x * cellSize, 0.05f, y * cellSize);
                Instantiate(gridVisualPrefab, position, Quaternion.identity);
            }
        }
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

    public void SetCellOccupied(Vector3 worldPosition)
    {
        Vector2Int cell = WorldToGrid(worldPosition);
        if (gridCells.ContainsKey(cell))
        {
            gridCells[cell] = true;
            Debug.Log($"Cell {cell} is now marked as filled.");
        }
    }

    public void SetCellAvailable(Vector3 worldPosition)
    {
        Vector2Int cell = WorldToGrid(worldPosition);
        if (gridCells.ContainsKey(cell))
        {
            gridCells[cell] = false;
            Debug.Log($"Cell {cell} has been marked as empty again.");
        }
    }

    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / cellSize);
        int y = Mathf.FloorToInt(worldPosition.z / cellSize);
        return new Vector2Int(x, y);
    }
}
