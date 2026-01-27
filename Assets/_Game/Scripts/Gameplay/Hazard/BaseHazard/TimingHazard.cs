using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TimingHazard : Hazard
{
    [Header("Timing Config")]
    [SerializeField] private float activeTime = 2f;   // Thời gian gây sát thương (Laser to)
    [SerializeField] private float warningTime = 1f;  // Thời gian cảnh báo (Laser nhỏ/nhấp nháy)
    [SerializeField] private float cooldownTime = 2f; // Thời gian nghỉ (Tắt hẳn)
    [SerializeField] private float startDelay = 0f;

    [Header("Events for Visuals (Animation/Scale/Particles)")]
    public UnityEvent OnWarning;   // Trigger khi bắt đầu cảnh báo (VD: Laser hiện mờ)
    public UnityEvent OnActivateTrigger;  // Trigger khi bật sát thương (VD: Laser to ra, Gai nhô lên)
    public UnityEvent OnDeactivate; // Trigger khi tắt (VD: Ẩn đi)

    [Header("Damage Config")]
    [SerializeField] private Collider2D damageCollider;

    private void Awake()
    {
        if (damageCollider == null)
        {
            damageCollider = GetComponent<Collider2D>();
            if (damageCollider == null)
            {
                Debug.LogError("⚠️ TimingHazard requires a Collider2D to function as damage collider.");
            }
        }

        // Vô hiệu hóa collider lúc đầu
        damageCollider.enabled = false;
    }

    protected virtual void Start()
    {
        StartCoroutine(TimingRoutine());
    }

    private IEnumerator TimingRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            // 1. Trạng thái nghỉ (Cooldown)
            damageCollider.enabled = false;
            OnDeactivate?.Invoke();
            yield return new WaitForSeconds(cooldownTime);

            // 2. Trạng thái cảnh báo (Warning)
            // Laser: Hiện tia nhỏ, chưa gây dame
            OnWarning?.Invoke();
            yield return new WaitForSeconds(warningTime);

            // 3. Trạng thái kích hoạt (Active)
            // Laser: Phình to, Bật collider
            damageCollider.enabled = true;
            OnActivateTrigger?.Invoke();
            yield return new WaitForSeconds(activeTime);
        }
    }

    protected override void OnActivate(PlayerEntity target)
    {
        base.OnActivate(target);
        target.Die();
    }
}