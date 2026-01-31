using UnityEngine;
using System.Collections;

public class SkillVFXController : MonoBehaviour
{
    [SerializeField] private GameObject skill1VFX;
    [SerializeField] private GameObject skill2VFX;
    [SerializeField] private GameObject skill3VFX;

    private Coroutine currentVFX;

    void OnEnable()
    {
        EventBus.On<FormChangeData>(FormEventType.OnFormChanged, OnFormChanged);
    }

    void OnDisable()
    {
        EventBus.Off<FormChangeData>(FormEventType.OnFormChanged, OnFormChanged);
    }

    private void OnFormChanged(FormChangeData data)
    {
        GameObject vfx = null;

        switch (data.ToFormID)
        {
            case 1: vfx = skill1VFX; break;
            case 2: vfx = skill2VFX; break;
            case 3: vfx = skill3VFX; break;
        }

        if (vfx == null) return;

        // 🔥 tránh spam coroutine
        if (currentVFX != null)
            StopCoroutine(currentVFX);

        currentVFX = StartCoroutine(PlayVFX(vfx));
    }

    private IEnumerator PlayVFX(GameObject vfx)
    {
        vfx.SetActive(true);

        yield return new WaitForSeconds(1f);

        vfx.SetActive(false);
    }
}
