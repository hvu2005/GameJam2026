using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardVisual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    // Lưu lại scale ban đầu để biết đường phóng to
    private Vector3 originalScale; 

    private void Awake() {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
    }

    public void ShowWarning() {
        gameObject.SetActive(true);
        transform.localScale = new Vector3(0.1f, originalScale.y, 1); // Teo nhỏ lại
    }

    public void ShowActive() {
        gameObject.SetActive(true);
        transform.localScale = originalScale; // To ra bình thường
    }

    public void Hide() {
        gameObject.SetActive(false); // Tắt đi
    }
}
