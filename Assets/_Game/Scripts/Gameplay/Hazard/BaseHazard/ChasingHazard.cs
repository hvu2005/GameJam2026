using UnityEngine;

public class ChasingHazard : Hazard {
    [Header("AI Config")]
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private float detectRange = 8f;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float chargeSpeed = 10f; // Tốc độ khi dơi lao vào
    [SerializeField] private LayerMask playerLayer;

    private Transform playerTransform;
    private AIState currentState = AIState.Idle;
    private Vector3 chargeDirection;

    private void Update() {
        switch (enemyType) {
            case EnemyType.HomingProjectile:
                HandleHoming();
                break;
            case EnemyType.Bat:
                HandleBatBehavior();
                break;
        }
    }

    // --- Logic Đạn Đuổi (Boss Skill) ---
    private void HandleHoming() {
        if (playerTransform == null) FindPlayer();
        else {
            // Di chuyển mượt về phía player
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
            
            // Xoay đầu về hướng di chuyển
            Vector3 dir = playerTransform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    // --- Logic Dơi (Game Jam specific) ---
    private void HandleBatBehavior() {
        switch (currentState) {
            case AIState.Idle:
                FindPlayer();
                if (playerTransform != null) currentState = AIState.Chasing;
                break;

            case AIState.Chasing:
                // Dơi bay ngang về phía player (giữ nguyên độ cao Y hoặc thay đổi chậm)
                float step = moveSpeed * Time.deltaTime;
                Vector3 targetPos = new Vector3(playerTransform.position.x, transform.position.y, 0); // Chỉ đuổi theo X
                transform.position = Vector2.MoveTowards(transform.position, targetPos, step);

                // Kiểm tra khoảng cách để lao vào (Charge)
                if (Vector2.Distance(transform.position, playerTransform.position) < 3f) {
                    chargeDirection = (playerTransform.position - transform.position).normalized;
                    currentState = AIState.Charging;
                }
                break;

            case AIState.Charging:
                // Lao thẳng theo hướng đã định, không đổi hướng nữa
                transform.position += chargeDirection * chargeSpeed * Time.deltaTime;
                break;
        }
    }

    private void FindPlayer() {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);
        if (hit) playerTransform = hit.transform;
    }
    
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}