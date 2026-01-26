using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledProjectile : Hazard
{
    [Header("Projectile Config")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifeTime = 3f; // Thời gian tự hủy nếu không trúng gì
    
    private ObjectPool myPool; // Tham chiếu để biết chui về đâu
    private float timer;

    // Hàm này được Spawner gọi ngay khi lấy đạn ra
    public void Initialize(ObjectPool pool)
    {
        myPool = pool;
        timer = 0f;
    }

    private void Update()
    {
        // 1. Logic di chuyển (Bay thẳng sang phải theo hướng của nó)
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // 2. Logic tự hủy theo thời gian (thay cho Destroy)
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            ReturnToPool();
        }
    }

    // Ghi đè hàm va chạm của Hazard
    protected override void ApplyEffect(IAffectable target)
    {
        // Gây sát thương như bình thường
        base.ApplyEffect(target);
        ReturnToPool();
    }

    // Xử lý va chạm với tường (Environment)
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground")) 
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (myPool != null)
        {
            myPool.Release(this.gameObject);
        }
        else
        {
            gameObject.SetActive(false); 
        }
    }
}
