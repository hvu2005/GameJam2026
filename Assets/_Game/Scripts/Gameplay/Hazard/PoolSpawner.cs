using UnityEngine;

public class PoolSpawner : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject prefabToSpawn; // Prefab viên đạn
    [SerializeField] private Transform spawnPoint;     // Vị trí bắn (miệng hoa)

    private ObjectPool bulletPool;

    private void Start()
    {
        // Lấy (hoặc tạo) Pool cho loại đạn này
        bulletPool = SingleBehaviour.Of<PoolController>().GetPool(prefabToSpawn);
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