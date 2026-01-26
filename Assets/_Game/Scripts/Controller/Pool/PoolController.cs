using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PoolItem
{
    public GameObject prefab;
    public int initialSize;
}

public sealed class PoolController : MonoBehaviour
{
    // Dùng prefab (GameObject reference) làm key thay vì Type
    private Dictionary<GameObject, ObjectPool> _pools = new();

    [SerializeField] private List<PoolItem> poolItems = new();

    void Awake()
    {
        foreach (var item in poolItems)
        {
            if (item.prefab == null) continue;
            
            var pool = new ObjectPool(item.prefab, item.initialSize);
            _pools[item.prefab] = pool;
        }
    }

    public void CreatePool(GameObject prefab, int initialSize = 2)
    {
        if (prefab == null || _pools.ContainsKey(prefab)) return;
        _pools[prefab] = new ObjectPool(prefab, initialSize);
    }

    public ObjectPool GetPool(GameObject prefab)
    {
        if (prefab == null) return null;
        
        if (_pools.TryGetValue(prefab, out ObjectPool pool))
        {
            return pool;
        }
        
        CreatePool(prefab, 2);
        return _pools[prefab];
    }
}
