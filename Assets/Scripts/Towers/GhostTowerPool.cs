using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectsType;

public class GhostTowerPool : MonoBehaviour {
    [Header("Fast Tower")]
    public Transform activeFastTower;
    public Transform deActiveFastTower;
    [Header("Slow Tower")]
    public Transform activeSlowTower;
    public Transform deActiveSlowTower;
    
    private GameObject _currentGhostInstance;

    public GameObject GetGhost(TowerData towerData) {
        if (_currentGhostInstance != null) ReturnCurrentGhost(towerData);
        
        Transform from = null;
        Transform to = null;
        switch (towerData.objectType)
        {
            case ObjectTypes.FastTower:
                from = deActiveFastTower;
                to = activeFastTower;
                break;
            case ObjectTypes.SlowTower:
                from = deActiveSlowTower;
                to = activeSlowTower;
                break;
        }

        if (from != null && from.childCount > 0) {
            Transform ghost = from.GetChild(0);
            ghost.SetParent(to);
            _currentGhostInstance = ghost.gameObject;
        }

        return _currentGhostInstance;

    }
    
    public void ReturnCurrentGhost(TowerData towerData)
    {
        if (_currentGhostInstance == null) return;

        Transform to = null;
        
        switch (towerData.objectType)
        {
            case ObjectTypes.FastTower:
                to = deActiveFastTower;
                break;
            case ObjectTypes.SlowTower:
                to = deActiveSlowTower;
                break;
        }

        if (to != null)
        {
            _currentGhostInstance.transform.SetParent(to);
        }

        _currentGhostInstance = null;
    }


}
