using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectsType;

public class GhostTowerPool : MonoBehaviour {
    [Header("Fast Tower")]
    public Transform activeFastTower; // Parent for active fast tower ghost
    public Transform deActiveFastTower; // Parent for inactive fast tower ghost
    [Header("Slow Tower")]
    public Transform activeSlowTower; // Parent for active slow tower ghost
    public Transform deActiveSlowTower; // Parent for inactive slow tower ghost
    private bool _isFastTower = false; // Tracks which tower type is currently active;
    private GameObject _currentGhostInstance; // Reference to the current active ghost instance

    public GameObject GetGhost(TowerData towerData) {
        // Return previous ghost if one exists
        if (_currentGhostInstance != null) ReturnCurrentGhost();
        
        Transform from = null;
        Transform to = null;
        
        // Determine parent transforms based on tower type
        switch (towerData.objectType) {
            case ObjectTypes.FastTower:
                from = deActiveFastTower;
                to = activeFastTower;
                _isFastTower = true;
                break;
            case ObjectTypes.SlowTower:
                from = deActiveSlowTower;
                to = activeSlowTower;
                _isFastTower = false;
                break;
        }

        // Move ghost from inactive pool to active scene
        if (from != null && from.childCount > 0) {
            Transform ghost = from.GetChild(0);
            ghost.SetParent(to);
            _currentGhostInstance = ghost.gameObject;
        }

        return _currentGhostInstance;

    }
    
    public void ReturnCurrentGhost()
    {
        if (_currentGhostInstance == null) return;

        // Return to corresponding inactive parent
        _currentGhostInstance.transform.SetParent(_isFastTower ? deActiveFastTower : deActiveSlowTower);

        _currentGhostInstance = null;
    }


}
