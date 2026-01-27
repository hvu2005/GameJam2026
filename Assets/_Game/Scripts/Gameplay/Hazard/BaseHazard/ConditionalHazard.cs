using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class ConditionalHazard : Hazard
{
    [Header("Conditional Settings")]
    [Tooltip("Thời gian chờ từ lúc phát hiện Player đến lúc bẫy hoạt động")]
    [SerializeField] protected float triggerDelay = 0.5f;

    [Tooltip("Nếu True: Bẫy chỉ kích hoạt 1 lần rồi vô hiệu hóa (VD: Nhũ băng rơi)")]
    [SerializeField] protected bool oneTimeUse = true;

    [Tooltip("Layer của Player để trigger nhận diện")]
    [SerializeField] protected LayerMask playerLayer;

    [Header("Events")]
    [Tooltip("Gọi khi bắt đầu đếm ngược (VD: Rung lắc cảnh báo)")]
    public UnityEvent OnWarning;
    
    [Tooltip("Gọi khi kết thúc đếm ngược (VD: Phát âm thanh tách/gãy)")]
    public UnityEvent OnActionExecuted;

    protected bool isTriggered = false;
    protected Coroutine triggerCoroutine;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggered && oneTimeUse) return;

        if (IsInLayerMask(other.gameObject, playerLayer))
        {
            if (!isTriggered)
            {
                TriggerHazard();
            }
            else
            {
                base.OnTriggerEnter2D(other); 
            }
        }
    }

    // Hàm tiện ích check Layer
    private bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return (mask.value & (1 << obj.layer)) > 0;
    }


    public void TriggerHazard()
    {
        if (triggerCoroutine != null) StopCoroutine(triggerCoroutine);
        triggerCoroutine = StartCoroutine(RoutineExecute());
    }

    private IEnumerator RoutineExecute()
    {
        isTriggered = true;

        // Giai đoạn 1: Cảnh báo (Warning)
        OnWarning?.Invoke();
        yield return new WaitForSeconds(triggerDelay);

        // Giai đoạn 2: Hành động (Action)
        OnActionExecuted?.Invoke();
        PerformHazardAction();
    }

    /// <summary>
    /// Hành động cụ thể của từng loại bẫy (Rơi, Trồi lên, Sập cầu...)
    /// </summary>
    protected abstract void PerformHazardAction();

    // Reset lại bẫy nếu cần
    public virtual void ResetHazard()
    {
        isTriggered = false;
        // Logic reset vị trí/trạng thái sẽ do class con tự xử lý
    }
}