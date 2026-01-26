using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PoolItem
{
    public string name;
    public GameObject prefab;
    public int initialSize;
}

public sealed class PoolController : MonoBehaviour
{
    // Dùng prefab (GameObject reference) làm key thay vì Type
    private Dictionary<string, ObjectPool> _pools = new();

    [SerializeField] private List<PoolItem> poolItems = new();

    void Awake()
    {
        foreach (var item in poolItems)
        {
            CreatePool(item);
        }
    }

    public void CreatePool(PoolItem item)
    {
        if (item.prefab == null || _pools.ContainsKey(item.name)) return;
        var pool = new ObjectPool(item.prefab, item.initialSize);
        _pools[item.name] = pool;
    }

    public ObjectPool GetPool(string name)
    {
        if (_pools.TryGetValue(name, out ObjectPool pool))
        {
            return pool;
        }

        throw new KeyNotFoundException($"Pool with name {name} not found.");
    }
}
