using UnityEngine;

namespace _Game.Scripts.Gameplay.Parallax
{
    public class ParallaxLayer : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Speed ratio relative to camera movement. 1 = move with camera (static), 0.5 = half speed, 0 = no movement (fixed to world).")]
        [SerializeField] private Vector2 parallaxFactor;
        
        [Tooltip("If true, the texture will repeat infinitely on the X axis.")]
        [SerializeField] private bool infiniteHorizontal;
        
        [Tooltip("If true, the texture will repeat infinitely on the Y axis.")]
        [SerializeField] private bool infiniteVertical;

        private Transform _cameraTransform;
        private Vector3 _lastCameraPosition;
        private float _textureUnitSizeX;
        private float _textureUnitSizeY;

        private void Start()
        {
            if (Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
                _lastCameraPosition = _cameraTransform.position;
            }
            else
            {
                Debug.LogError("[ParallaxLayer] Main Camera not found!");
                enabled = false;
                return;
            }

            // Calculate texture size for infinite scrolling
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && (infiniteHorizontal || infiniteVertical))
            {
                Texture2D texture = spriteRenderer.sprite.texture;
                _textureUnitSizeX = texture.width / spriteRenderer.sprite.pixelsPerUnit;
                _textureUnitSizeY = texture.height / spriteRenderer.sprite.pixelsPerUnit;
                
                // Adjust for scale
                _textureUnitSizeX *= transform.lossyScale.x;
                _textureUnitSizeY *= transform.lossyScale.y;
            }
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null) return;

            Vector3 deltaMovement = _cameraTransform.position - _lastCameraPosition;
            
            // Calculate new position
            // If factor is 1, it moves exactly with camera (appears static relative to camera).
            // If factor is 0, it doesn't move (static relative to world).
            // Usually for background we want factor < 1 (e.g. 0.5) to move slower than cam.
            
            // Standard Parallax Formula: 
            // We want to move the object by (1 - factor) * delta
            // Wait, let's re-verify:
            // If Cam moves +10, and we want object to appear "far away", it should traverse LESS screen space.
            // If object stays at 0, it moves -10 relative to Cam.
            // If object moves +5, it moves -5 relative to Cam (0.5 factor).
            // So we move the object by (factor * delta).
            
            transform.position += new Vector3(deltaMovement.x * parallaxFactor.x, deltaMovement.y * parallaxFactor.y, 0f);
            
            _lastCameraPosition = _cameraTransform.position;

            // Infinite Scrolling Check
            if (infiniteHorizontal)
            {
                if (Mathf.Abs(_cameraTransform.position.x - transform.position.x) >= _textureUnitSizeX)
                {
                    float offsetPositionX = (_cameraTransform.position.x - transform.position.x) % _textureUnitSizeX;
                    transform.position = new Vector3(_cameraTransform.position.x + offsetPositionX, transform.position.y, transform.position.z);
                }
            }
            
            if (infiniteVertical)
            {
                if (Mathf.Abs(_cameraTransform.position.y - transform.position.y) >= _textureUnitSizeY)
                {
                    float offsetPositionY = (_cameraTransform.position.y - transform.position.y) % _textureUnitSizeY;
                    transform.position = new Vector3(transform.position.x, _cameraTransform.position.y + offsetPositionY, transform.position.z);
                }
            }
        }
    }
}
