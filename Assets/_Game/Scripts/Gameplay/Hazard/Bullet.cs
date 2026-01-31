using UnityEngine;

/// <summary>
/// Bullet - Đạn bay theo hướng và gây sát thương khi chạm Player
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : Hazard
{
    [Header("Bullet Config")]
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private bool destroyOnHit = true;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float speed;
    private float spawnTime;

    // Callback để trả bullet về pool
    public System.Action OnBulletDestroyed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        Debug.Log($"[Bullet] Awake called on {gameObject.name}");
    }
    
    private void OnEnable()
    {
        Debug.Log($"[Bullet] OnEnable called on {gameObject.name}, active: {gameObject.activeSelf}, spawnTime: {spawnTime}, currentTime: {Time.time}");
    }
    
    private void OnDisable()
    {
        Debug.Log($"[Bullet] OnDisable called on {gameObject.name}");
    }

    /// <summary>
    /// Initialize bullet với direction và speed
    /// Gọi method này từ Turret sau khi spawn
    /// </summary>
    public void Initialize(Vector2 direction, float bulletSpeed)
    {
        // Reset velocity
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        
        // Set direction và speed
        moveDirection = direction.normalized;
        speed = bulletSpeed;
        
        // QUAN TRỌNG: Reset spawn time mỗi lần initialize
        spawnTime = Time.time;

        // Set velocity
        rb.velocity = moveDirection * speed;
        
        // Set rotation theo hướng bay
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        Debug.Log($"Bullet initialized at {Time.time}, lifetime will end at {spawnTime + lifetime}");
    }

    private void Update()
    {
        // Tự hủy sau lifetime
        if (Time.time - spawnTime >= lifetime)
        {
            DestroyBullet();
        }
    }

    /// <summary>
    /// Override OnActivate để gây sát thương khi chạm Player
    /// </summary>
    protected override void OnActivate(PlayerEntity target)
    {
        base.OnActivate(target);

        target.Die();

        // Spawn hit effect
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        // Emit event nếu cần
        // EventBus.Emit<Vector2>(GameEvent.BulletHit, transform.position);

        // Hủy đạn sau khi chạm
        if (destroyOnHit)
        {
            DestroyBullet();
        }
    }

    /// <summary>
    /// Xử lý va chạm với tường hoặc obstacle
    /// </summary>
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        // Nếu chạm vào tường, cũng hủy đạn
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, collision.contacts[0].point, Quaternion.identity);
            }

            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        // Reset velocity
        rb.velocity = Vector2.zero;

        // Gọi callback (pool sẽ Release)
        if (OnBulletDestroyed != null)
        {
            OnBulletDestroyed.Invoke();
        }
        else
        {
            // Fallback: Destroy nếu không dùng pool
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && rb != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)rb.velocity.normalized * 2f);
        }
    }
}
