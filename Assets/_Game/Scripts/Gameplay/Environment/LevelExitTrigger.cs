using _Game.Scripts.Tool;
using UnityEngine;

namespace _Game.Scripts.Gameplay.Environment
{
    [RequireComponent(typeof(Collider2D))]
    public class LevelExitTrigger : MonoBehaviour
    {
        private bool _isTriggered = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isTriggered) return;

            // Check if object is Player
            // Assuming Player has "Player" tag, or we check for script
            if (other.CompareTag("Player") || other.GetComponent<Player>() != null)
            {
                TriggerNextLevel();
            }
        }

        private void TriggerNextLevel()
        {
            _isTriggered = true;
            Debug.Log("[LevelExitTrigger] Player reached exit! Loading next level...");

            SimpleMapLoader mapLoader = FindObjectOfType<SimpleMapLoader>();
            if (mapLoader != null)
            {
                mapLoader.LoadNextLevel();
            }
            else
            {
                Debug.LogError("[LevelExitTrigger] SimpleMapLoader not found in scene!");
            }
        }
    }
}
