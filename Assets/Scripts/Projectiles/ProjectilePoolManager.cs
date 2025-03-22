using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ProjectilePoolManager : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int initialPoolSize = 20;
    public Transform activeParent;
    public Transform deactiveParent;

    private readonly Queue<GameObject> _pool = new();

    private void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject proj = Instantiate(projectilePrefab, deactiveParent);
            _pool.Enqueue(proj);
        }
    }

    public GameObject GetProjectile()
    {
        GameObject proj = _pool.Count > 0 ? _pool.Dequeue() : Instantiate(projectilePrefab);
        proj.transform.SetParent(activeParent);
        return proj;
    }

    public void PoolProjectile(GameObject proj)
    {
        proj.transform.SetParent(deactiveParent);
        _pool.Enqueue(proj);
    }
}