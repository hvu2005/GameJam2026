using UnityEngine;

namespace _Game.Scripts.Gameplay.Parallax
{
    /// <summary>
    /// Automatically scales the Sprite to cover the Camera view on Awake.
    /// Useful for backgrounds that need to fit the screen exactly or be slightly larger.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteFitToCamera : MonoBehaviour
    {
        [Tooltip("If true, maintains aspect ratio and uses 'Envelope/Cover' mode (crops edges if needed to fill). If false, stretches to fit exactly.")]
        [SerializeField] private bool keepAspectRatio = true;
        
        [Tooltip("Multiplier for the final size. Set > 1 if you want the background to be larger than the screen (useful for Parallax scrolling).")]
        [SerializeField] private float sizeMultiplier = 1.0f;

        private void Awake()
        {
            FitToCamera();
        }

        [ContextMenu("Apply Fit Now")]
        public void FitToCamera()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr == null || Camera.main == null) return;
            
            // 1. Get Camera World Size
            float cameraHeight = Camera.main.orthographicSize * 2;
            float cameraWidth = cameraHeight * Camera.main.aspect;
            Vector2 cameraSize = new Vector2(cameraWidth, cameraHeight);
            
            if (sr.sprite == null) return;
            Vector2 spriteSize = sr.sprite.bounds.size; 
    
            
            float targetScaleX = cameraSize.x / spriteSize.x;
            float targetScaleY = cameraSize.y / spriteSize.y;
            
            Vector3 finalScale = transform.localScale;

            if (keepAspectRatio)
            {
                // To COVER the screen (Envelope), take the larger scale
                float maxScale = Mathf.Max(targetScaleX, targetScaleY);
                finalScale.x = maxScale;
                finalScale.y = maxScale;
            }
            else
            {
                // Stretch to fill
                finalScale.x = targetScaleX;
                finalScale.y = targetScaleY;
            }
            
            // Apply Multiplier
            finalScale *= sizeMultiplier;
            finalScale.z = 1f; // Keep Z scale 1

            transform.localScale = finalScale;
        }
    }
}
