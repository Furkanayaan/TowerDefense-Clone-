using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class EnemyRoadPoints : MonoBehaviour {
    [Inject] private GridManager _gridManager;
    public Transform initTransform;
    public Transform targetTransform;
    void Start() {
        float cameraXPos = (_gridManager.maksCellOnRow - 1) * _gridManager.cellSize / 2f;
        initTransform.position = new Vector3(cameraXPos, initTransform.position.y,
            initTransform.position.z);
        
        targetTransform.position = new Vector3(cameraXPos, targetTransform.position.y,
            targetTransform.position.z);
    }

    public Transform GetInitTransform() {
        return initTransform;
    }
    
    public Transform GetTargetTransform() {
        return targetTransform;
    }
}
