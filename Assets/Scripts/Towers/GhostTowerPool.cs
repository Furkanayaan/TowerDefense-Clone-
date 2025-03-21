using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowersType;

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
        switch (towerData.towerType)
        {
            case TowerTypes.Fast:
                from = deActiveFastTower;
                to = activeFastTower;
                break;
            case TowerTypes.Slow:
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
        
        switch (towerData.towerType)
        {
            case TowerTypes.Fast:
                to = deActiveFastTower;
                break;
            case TowerTypes.Slow:
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
