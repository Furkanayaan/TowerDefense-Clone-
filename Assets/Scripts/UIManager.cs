using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UIManager : MonoBehaviour
{
    [Inject] private GridManager _gridManager;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCell() {
        _gridManager.cellCount[_gridManager.rowCount]++;
        if (_gridManager.cellCount[_gridManager.rowCount] >= _gridManager.maksCellOnRow) {
            _gridManager.rowCount++;
            _gridManager.cellCount.Add(0);
        }
        _gridManager.GenerateGrid();
        
    }
}
