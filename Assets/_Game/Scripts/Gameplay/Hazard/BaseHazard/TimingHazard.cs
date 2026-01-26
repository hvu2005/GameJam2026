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
    public UnityEvent OnActivate;  // Trigger khi bật sát thương (VD: Laser to ra, Gai nhô lên)
    public UnityEvent OnDeactivate; // Trigger khi tắt (VD: Ẩn đi)

    [Header("VFX")]
    [SerializeField] protected ParticleSystem hitVFX;

    private void Start()
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
            OnActivate?.Invoke();
            yield return new WaitForSeconds(activeTime);
        }
    }

    protected override void ApplyEffect(IAffectable target)
    {
        base.ApplyEffect(target);
        hitVFX?.Play();
    }
}