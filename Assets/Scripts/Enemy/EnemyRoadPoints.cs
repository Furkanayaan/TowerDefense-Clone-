using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class EnemyRoadPoints : MonoBehaviour {
    [Inject] private GridManager _gridManager;
    
    // Start and end points for enemy movement
    public Transform initTransform;
    public Transform targetTransform;
    void Start() {
        // Calculate half the grid width in world units
        float halfGridWidth = (_gridManager.maksCellOnRow - 1) * _gridManager.cellSize * 0.5f;

        // Align both transforms to the grid center on the X axis
        SetXPosition(initTransform, halfGridWidth);
        SetXPosition(targetTransform, halfGridWidth);
    }
    private void SetXPosition(Transform t, float x)
    {
        Vector3 pos = t.position;
        pos.x = x;
        t.position = pos;
    }

    public Transform GetInitTransform() {
        return initTransform;
    }
    
    public Transform GetTargetTransform() {
        return targetTransform;
    }
}
