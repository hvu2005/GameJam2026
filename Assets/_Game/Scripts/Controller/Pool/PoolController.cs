using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PoolItem
{
    public string name;
    public GameObject prefab;
    public int initialSize;
}

public class PoolController : MonoBehaviour
{
    private static PoolController _instance;
    
    public static PoolController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PoolController>();
            }
            return _instance;
        }
    }
    
    // Dùng prefab (GameObject reference) làm key thay vì Type
    private Dictionary<string, ObjectPool> _pools = new();

    [SerializeField] private List<PoolItem> poolItems = new();

    void Awake()
    {
        // Singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        foreach (var item in poolItems)
        {
            CreatePool(item);
        }

        DontDestroyOnLoad(this.gameObject);
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
