using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ProjectilePoolManager : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab to be pooled
    public int initialPoolSize = 20; // Number of projectiles to pre-instantiate
    public Transform activeParent; // Parent for active projectiles
    public Transform deactiveParent; // Parent for pooled (inactive) projectiles

    private Dictionary<GameObject, Projectile> _projectileLookup = new(); // Cache for quick access to Projectile scripts

    private readonly Queue<GameObject> _pool = new(); // Queue to manage pooled projectiles

    private void Awake()
    {
        // Pre-instantiate projectiles and store them in the pool
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab, deactiveParent);
            _pool.Enqueue(projectile);
            Projectile proj = projectile.GetComponent<Projectile>();
            _projectileLookup[projectile] = proj;
        }
    }

    public GameObject GetPooledProjectile()
    {
        GameObject proj;
        // Get a projectile from the pool or instantiate a new one if pool is empty
        if (_pool.Count > 0)
        {
            proj = _pool.Dequeue();
        }
        else
        {
            proj = Instantiate(projectilePrefab);
            Projectile projScript = proj.GetComponent<Projectile>();
            _projectileLookup[proj] = projScript;
        }

        proj.transform.SetParent(activeParent);
        return proj;
    }

    public void ReturnToPool(GameObject proj)
    {
        // Return projectile to the pool and set it as inactive
        proj.transform.SetParent(deactiveParent);
        _pool.Enqueue(proj);
    }

    public Projectile ReturnProjectileScript(GameObject projectileGameObject)
    {
        // Retrieve the cached Projectile script if available
        if (_projectileLookup.TryGetValue(projectileGameObject, out var projectile))
            return projectile;
        
        return null;
    }
}