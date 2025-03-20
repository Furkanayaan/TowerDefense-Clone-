using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisual : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 2f;
    public GameObject gridPrefab;

    private void Start()
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * cellSize, 0.1f, y * cellSize);
                Instantiate(gridPrefab, position, Quaternion.identity);
            }
        }
    }
}
