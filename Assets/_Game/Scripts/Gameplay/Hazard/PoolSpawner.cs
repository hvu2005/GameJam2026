using UnityEngine;

public class PoolSpawner : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private string poolName;          // Tên pool để lấy từ PoolController
    [SerializeField] private Transform spawnPoint;     // Vị trí bắn (miệng hoa)

    private ObjectPool bulletPool;

    private void Start()
    {
        // Lấy Pool theo tên từ PoolController
        if (string.IsNullOrEmpty(poolName))
        {
            Debug.LogError("[PoolSpawner] Pool name is empty! Please set poolName in Inspector.");
            return;
        }

        try
        {
            bulletPool = SingleBehaviour.Of<PoolController>().GetPool(poolName);
        }
        catch (System.Collections.Generic.KeyNotFoundException)
        {
            Debug.LogError($"[PoolSpawner] Pool '{poolName}' not found in PoolController!");
        }
    }

    public void Spawn()
    {
        if (bulletPool == null) return;

        // 1. Lấy đạn từ Pool (thay vì Instantiate)
        GameObject bulletObj = bulletPool.Get();

        // 2. Đặt vị trí và hướng bắn
        bulletObj.transform.position = spawnPoint.position;
        bulletObj.transform.rotation = spawnPoint.rotation;

        // 3. Setup tham chiếu Pool cho viên đạn (để nó biết đường quay về)
        if (bulletObj.TryGetComponent<PooledProjectile>(out var projectile))
        {
            projectile.Initialize(bulletPool);
        }
    }
}