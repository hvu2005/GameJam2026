using _Game.Scripts.Tool;
using System.Collections;
using UnityEngine;

namespace _Game.Scripts.Gameplay.Environment
{
    [RequireComponent(typeof(Collider2D))]
    public class LevelExitTrigger : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private Animator animator;
        [SerializeField] private string idleTriggerName = "Idle";
        [SerializeField] private string openDoorTriggerName = "OpenDoor";
        [SerializeField] private string openDoorStateName = "Door"; // Tên state trong Animator
        
        private bool _isTriggered = false;

        private void Awake()
        {
            // Auto-get Animator nếu chưa assign
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isTriggered) return;

            // Check if object is Player
            if (other.CompareTag("Player") || other.GetComponent<Player>() != null)
            {
                StartCoroutine(TriggerNextLevelWithAnimation());
            }
        }

        /// <summary>
        /// Chạy animation OpenDoor trước, đợi animation chạy xong rồi mới load level tiếp theo
        /// </summary>
        private IEnumerator TriggerNextLevelWithAnimation()
        {
            _isTriggered = true;
            Debug.Log("[LevelExitTrigger] Player reached exit! Playing door animation...");

            // Chạy animation OpenDoor
            if (animator != null)
            {
                animator.ResetTrigger(idleTriggerName);
                animator.SetTrigger(openDoorTriggerName);

                // Đợi animation OpenDoor chạy xong
                yield return new WaitForSeconds(GetAnimationDuration());

                Debug.Log("[LevelExitTrigger] Door animation completed!");
            }

            // Load next level sau khi animation xong
            SimpleMapLoader mapLoader = FindObjectOfType<SimpleMapLoader>();
            if (mapLoader != null)
            {
                mapLoader.LoadNextLevel();
            }
            else
            {
                Debug.LogError("[LevelExitTrigger] SimpleMapLoader not found in scene!");
            }

            // Reset về Idle state an toàn
            ResetToIdle();
        }

        /// <summary>
        /// Lấy thời gian của animation OpenDoor
        /// </summary>
        private float GetAnimationDuration()
        {
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                return 0f;
            }

            // Tìm animation clip theo tên state
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name.Contains(openDoorStateName))
                {
                    Debug.Log($"[LevelExitTrigger] Found animation '{clip.name}' with duration: {clip.length}s");
                    return clip.length;
                }
            }

            Debug.LogWarning($"[LevelExitTrigger] Animation '{openDoorStateName}' not found! Using default 1s");
            return 1f; // Fallback duration
        }

        /// <summary>
        /// Reset Animator về Idle state an toàn
        /// </summary>
        private void ResetToIdle()
        {
            if (animator != null)
            {
                animator.ResetTrigger(openDoorTriggerName);
                animator.SetTrigger(idleTriggerName);
                animator.Update(0f); // Force update Animator
                Debug.Log("[LevelExitTrigger] Reset to Idle state");
            }
        }
    }
}
