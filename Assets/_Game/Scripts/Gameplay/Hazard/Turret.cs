using UnityEngine;

/// <summary>
/// Turret - Súng tự động bắn đạn theo chu kỳ
/// </summary>
public class Turret : TimingHazard
{
    [Header("Turret Config")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private Vector2 fireDirection = Vector2.right;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string shootTrigger = "Shoot";

    [Header("Pool Settings")]
    [SerializeField] private string bulletPoolName = "Bullet";

    private ObjectPool bulletPool;
    private PoolController poolController;

    private void Awake()
    {
        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    protected override void Start()
    {
        base.Start();
        poolController = FindObjectOfType<PoolController>();

        if (poolController != null)
        {
            try
            {
                bulletPool = poolController.GetPool(bulletPoolName);
                Debug.Log($"Turret: Bullet pool '{bulletPool}' initialized.");
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                Debug.LogError($"⚠️ Pool '{bulletPoolName}' không tồn tại! Hãy thêm vào PoolController trong Inspector.");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Không tìm thấy PoolController trong scene!");
        }
        OnActivateTrigger.AddListener(ShootBullet);
    }
    public void ShootBullet()
    {
        Debug.Log("Turret: Shooting bullet");
        
        // Trigger animation
        if (animator != null && !string.IsNullOrEmpty(shootTrigger))
        {
            animator.SetTrigger(shootTrigger);
        }
        
        if (bulletPool == null)
        {
            Debug.LogError("⚠️ Bullet pool chưa được khởi tạo!");
            return;
        }

        GameObject bulletObj = bulletPool.Get();
        
        // Kiểm tra bullet object
        if (bulletObj == null)
        {
            Debug.LogError("⚠️ Bullet pool không trả về object (có thể pool rỗng)!");
            return;
        }

        Debug.Log($"Turret: Bullet retrieved from pool, active state: {bulletObj.activeSelf}");

        // Set position và rotation TRƯỚC khi initialize
        bulletObj.transform.position = firePoint.position;
        bulletObj.transform.rotation = firePoint.rotation;

        Collider2D turretCol = GetComponent<Collider2D>();
        Collider2D bulletCol = bulletObj.GetComponent<Collider2D>();

        if (turretCol != null && bulletCol != null)
        {
            Physics2D.IgnoreCollision(turretCol, bulletCol, true);
        }

        // Configure bullet
        if (bulletObj.TryGetComponent<Bullet>(out var bullet))
        {
            bullet.Initialize(fireDirection.normalized, bulletSpeed);

            // Setup callback để return bullet về pool
            bullet.OnBulletDestroyed = () => bulletPool.Release(bulletObj);
        }
        else
        {
            Debug.LogWarning("⚠️ Bullet prefab không có component Bullet!");
        }
    }

    private void OnDestroy()
    {
        OnActivateTrigger.RemoveListener(ShootBullet);
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint == null) firePoint = transform;

        Gizmos.color = Color.red;
        Vector3 direction = fireDirection.normalized;
        Gizmos.DrawRay(firePoint.position, direction * 2f);
        Gizmos.DrawWireSphere(firePoint.position + (Vector3)direction * 2f, 0.2f);
    }
}
