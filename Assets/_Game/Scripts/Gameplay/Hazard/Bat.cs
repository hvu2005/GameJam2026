using UnityEngine;

/// <summary>
/// Bat - Con dơi với behavior đặc biệt:
/// 1. Ban đầu treo ngược 180° và đứng yên
/// 2. Khi detect player → đuổi ngang theo player (chỉ di chuyển theo trục X)
/// 3. Khi gần player → lao thẳng ngang với tốc độ cao
/// 4. Player nhảy lên là né được vì dơi bay ngang
/// </summary>
public class Bat : ChasingHazard
{
    [Header("Bat Behavior")]
    [Tooltip("Khoảng cách để trigger charge (lao thẳng)")]
    [SerializeField] private float chargeDistance = 3f;
    
    [Tooltip("Tốc độ khi lao thẳng")]
    [SerializeField] private float chargeSpeed = 10f;
    
    [Tooltip("Khoảng cách tối đa bay xa trước khi quay lại")]
    [SerializeField] private float maxChargeDistance = 10f;
    
    [Tooltip("Thời gian tối đa charge trước khi quay lại (giây)")]
    [SerializeField] private float maxChargeTime = 2f;

    private BatState currentState = BatState.Idle;
    private Vector3 chargeDirection;
    private Vector3 chargeStartPosition;
    private float chargeStartTime;
    private Vector3 idlePosition;

    private void Start()
    {
        // Lưu vị trí ban đầu
        idlePosition = transform.position;
        
        // Treo ngược 180 độ như dơi
        transform.rotation = Quaternion.Euler(0, 0, 180);
    }

    protected override void Update()
    {
        // Xử lý state theo behavior của dơi
        switch (currentState)
        {
            case BatState.Idle:
                HandleIdle();
                break;
            case BatState.Chasing:
                HandleChasing();
                break;
            case BatState.Charging:
                HandleCharging();
                break;
            case BatState.Returning:
                HandleReturning();
                break;
        }
    }

    /// <summary>
    /// Trạng thái đứng yên, treo ngược và chờ detect player
    /// </summary>
    private void HandleIdle()
    {
        FindPlayer();
        
        // Nếu phát hiện player, chuyển sang trạng thái đuổi
        if (playerTransform != null)
        {
            currentState = BatState.Chasing;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void HandleChasing()
    {
        if (playerTransform == null) 
        {
            // Nếu mất player, quay về trạng thái idle
            currentState = BatState.Idle;
            transform.position = idlePosition;
            transform.rotation = Quaternion.Euler(0, 0, 180);
            return;
        }

        Vector3 targetPos = new Vector3(
            playerTransform.position.x, 
            transform.position.y,
            transform.position.z
        );
        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetPos, 
            moveSpeed * Time.deltaTime
        );

        if (playerTransform.position.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1); // Quay trái
        else
            transform.localScale = new Vector3(1, 1, 1);  // Quay phải

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer < chargeDistance)
        {
            chargeDirection = new Vector3(
                Mathf.Sign(playerTransform.position.x - transform.position.x), 
                0, 
                0
            ).normalized;
            chargeStartPosition = transform.position;
            chargeStartTime = Time.time;
            currentState = BatState.Charging;
        }
    }

    private void HandleCharging()
    {
        transform.position += chargeDirection * chargeSpeed * Time.deltaTime;
        
        float distanceTraveled = Vector3.Distance(transform.position, chargeStartPosition);
        float timeElapsed = Time.time - chargeStartTime;

        if (distanceTraveled > maxChargeDistance || 
            timeElapsed > maxChargeTime || 
            Mathf.Abs(transform.position.x) > 20f)
        {
            // Chuyển sang trạng thái bay về
            currentState = BatState.Returning;
            playerTransform = null;
        }
    }

    private void HandleReturning()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            idlePosition,
            moveSpeed * Time.deltaTime
        );

        if (idlePosition.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);

        if (Vector3.Distance(transform.position, idlePosition) < 0.1f)
        {
            transform.position = idlePosition;
            transform.rotation = Quaternion.Euler(0, 0, 180);
            currentState = BatState.Idle;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chargeDistance);

        if (Application.isPlaying && playerTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
        
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(idlePosition, 0.3f);
        }
    }

    private enum BatState
    {
        Idle,      // Treo ngược, đứng yên
        Chasing,   // Đuổi ngang theo player
        Charging,  // Lao thẳng ngang
        Returning  // Bay về vị trí ban đầu
    }
}
