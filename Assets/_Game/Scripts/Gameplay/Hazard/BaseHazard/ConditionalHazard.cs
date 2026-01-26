using System.Collections;
using UnityEngine;

public class ConditionalHazard : Hazard
{
    [Header("Condition Config")]
    [SerializeField] private TriggerType triggerType;
    [SerializeField] private ActionType actionType;
    [SerializeField] private float triggerDelay = 0.5f; // Thời gian rung lắc trước khi rơi

    [Header("Detection (Proximity)")]
    [SerializeField] private Vector2 detectionBoxSize = new Vector2(1, 10); // Cho nhũ băng (quét dọc xuống)
    [SerializeField] private Vector2 detectionOffset = new Vector2(0, -5);
    [SerializeField] private LayerMask playerLayer;

    [Header("Physics (Falling)")]
    [SerializeField] private Rigidbody2D rb;

    private bool isTriggered = false;

    private void Update()
    {
        if (isTriggered || triggerType != TriggerType.Proximity) return;

        // Quét vùng phát hiện (Dùng cho Nhũ băng hoặc Cóc)
        if (Physics2D.OverlapBox((Vector2)transform.position + detectionOffset, detectionBoxSize, 0, playerLayer))
        {
            StartCoroutine(ExecuteAction());
        }
    }

    // Dùng cho Cầu gỗ (TriggerType = Contact)
    // Cần attach script này vào object cầu và set Collider là Trigger hoặc dùng OnCollisionEnter2D
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!isTriggered && triggerType == TriggerType.Contact && other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(ExecuteAction());
        }
    }

    private IEnumerator ExecuteAction()
    {
        isTriggered = true;

        // 1. Giai đoạn Warning (Rung lắc)
        float timer = 0;
        Vector3 startPos = transform.position;
        while (timer < triggerDelay)
        {
            transform.position = startPos + (Vector3)(Random.insideUnitCircle * 0.1f); // Rung nhẹ
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;

        // 2. Giai đoạn Action
        switch (actionType)
        {
            case ActionType.Fall: // Nhũ băng
                if (rb)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.gravityScale = 3f;
                }
                break;

            case ActionType.AppearAttack: // Cóc / Cây đuôi cáo
                // Bật collider sát thương
                damageCollider.enabled = true;
                // Chạy animation trồi lên
                // Animator.SetTrigger("Attack"); 
                break;

            case ActionType.Break: // Cầu gỗ gãy
                // Disable visual và collider đứng
                GetComponent<Collider2D>().enabled = false; // Player rơi xuống
                // Play break VFX
                Destroy(gameObject, 2f);
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (triggerType == TriggerType.Proximity)
            Gizmos.DrawWireCube((Vector2)transform.position + detectionOffset, detectionBoxSize);
    }
}