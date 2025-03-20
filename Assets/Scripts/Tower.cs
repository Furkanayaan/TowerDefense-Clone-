using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Tower : MonoBehaviour
{
    private GridManager _gridManager;
    private Vector3 _position;

    [Inject]
    public void Initialize(GridManager gridManager)
    {
        _gridManager = gridManager;
        _position = transform.position;
    }

    private void OnMouseDown()
    {
        SellTower();
    }

    private void SellTower()
    {
        Debug.Log("Tower has been sold!");
        _gridManager.SetCellAvailable(_position);
        Destroy(gameObject);
    }
    public class Factory : PlaceholderFactory<Tower> { }
}
