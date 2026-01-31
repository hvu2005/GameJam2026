using UnityEngine;
using UnityEngine.InputSystem;
using _Game.Scripts._Config.Data;
using Cinemachine;

namespace _Game.Scripts.Tool
{
    public class SimpleMapLoader : MonoBehaviour
    {
        [Header("Map Database")]
        [SerializeField] private MapDatabase mapDatabase;
        
        [Header("UI References")]
        [SerializeField] private UnityEngine.UI.Image uiBackgroundImage;

        [Header("Player Settings")]
        [Tooltip("Tự động telemetry player đến Spawn Point khi load map")]
        [SerializeField] private bool teleportPlayerOnLoad = true;
        
        [Header("Settings")]
        [SerializeField] private Transform spawnPoint; // Override spawn point location only if needed, mostly we use 'PlayerSpawn' tag in map
        [SerializeField] private bool autoLoadOnStart = true;
        [SerializeField] private bool showDebugInfo = true;

        private const string PLAYER_PREFS_LEVEL_KEY = "CurrentLevelIndex";
        
        private GameObject _currentMapInstance;
        private GameObject _currentPlayerInstance;
        private int _currentLevelIndex = 0;
        
        private void Start()
        {
            if (mapDatabase == null)
            {
                Debug.LogError("[SimpleMapLoader] MapDatabase Missing!");
                return;
            }
            
            if (autoLoadOnStart)
            {
                _currentLevelIndex = PlayerPrefs.GetInt(PLAYER_PREFS_LEVEL_KEY, 0);
                
                if (_currentLevelIndex >= mapDatabase.GetTotalMaps())
                {
                    _currentLevelIndex = 0; 
                }
                
                LoadLevel(_currentLevelIndex);
            }
        }
        
        private void Update()
        {
            #if UNITY_EDITOR
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                ReloadCurrentLevel();
            }
            if (Keyboard.current != null && Keyboard.current.nKey.wasPressedThisFrame)
            {
               LoadNextLevel();
            }
            #endif
        }
        
        public void LoadLevel(int index)
        {
            if (mapDatabase == null) return;
            
            if (index < 0 || index >= mapDatabase.GetTotalMaps())
            {
                Debug.LogError($"[SimpleMapLoader] Invalid level index: {index}");
                return;
            }

            UnloadCurrentMap();
            
            LevelData data = mapDatabase.GetLevelData(index);
            if (data == null) return;

            if (data.mapPrefab != null)
            {
                _currentMapInstance = Instantiate(data.mapPrefab, Vector3.zero, Quaternion.identity);
                _currentMapInstance.name = $"Map_{data.levelName}";
            }
            else
            {
                Debug.LogError($"[SimpleMapLoader] Map Prefab missing for level {index}");
            }

            // Set UI Background
            if (uiBackgroundImage != null)
            {
                if (data.backgroundSprite != null)
                {
                    uiBackgroundImage.sprite = data.backgroundSprite;
                    uiBackgroundImage.gameObject.SetActive(true);
                }
                else
                {
                    uiBackgroundImage.gameObject.SetActive(false); // Hide if no BG
                }
            }
            else
            {
                if (data.backgroundSprite != null)
                    Debug.LogWarning("[SimpleMapLoader] Background Sprite found but 'Ui Background Image' is not assigned!");
            }

            if (teleportPlayerOnLoad)
            {
                TeleportPlayerToSpawnPoint();
            }
            
            _currentLevelIndex = index;
            
            if (showDebugInfo)
            {
                Debug.Log($"[SimpleMapLoader] Loaded Level {index}: {data.levelName}");
            }
        }
        
        public void LoadNextLevel()
        {
            int nextIndex = _currentLevelIndex + 1;
            
            if (nextIndex >= mapDatabase.GetTotalMaps())
            {
                Debug.Log("[SimpleMapLoader] All levels completed! Looping or Ending...");
                nextIndex = 0; 
            }

            SaveProgress(nextIndex);
            LoadLevel(nextIndex);
        }

        public void ReloadCurrentLevel()
        {
            LoadLevel(_currentLevelIndex);
        }
        
        private void SaveProgress(int index)
        {
            PlayerPrefs.SetInt(PLAYER_PREFS_LEVEL_KEY, index);
            PlayerPrefs.Save();
        }

        public void ResetProgress()
        {
            SaveProgress(0);
            LoadLevel(0);
        }

        private void TeleportPlayerToSpawnPoint()
        {
            Transform spawnTransform = null;

            if (_currentMapInstance != null)
            {
                foreach (Transform t in _currentMapInstance.GetComponentsInChildren<Transform>(true))
                {
                    if (t.CompareTag("PlayerSpawn"))
                    {
                        spawnTransform = t;
                        break;
                    }
                }
            }

            if (spawnTransform == null)
            {
                 GameObject spawnPointObj = GameObject.FindGameObjectWithTag("PlayerSpawn");
                 if (spawnPointObj != null) spawnTransform = spawnPointObj.transform;
            }
            
            Vector3 targetPosition = Vector3.zero;
            int facingDirection = 1;

            if (spawnTransform != null)
            {
                targetPosition = spawnTransform.position;
                facingDirection = spawnTransform.localScale.x >= 0 ? 1 : -1;
            }
            else
            {
                if (showDebugInfo) Debug.LogWarning("[SimpleMapLoader] 'PlayerSpawn' tag not found. Using Zero.");
                if (spawnPoint) targetPosition = spawnPoint.position;
            }
            
            Player player = FindObjectOfType<Player>();
            
            if (player == null)
            {
                Debug.LogError("[SimpleMapLoader] Player not found in scene! Ensure Player is present in the scene.");
                return;
            }
            
            _currentPlayerInstance = player.gameObject;

            player.transform.position = targetPosition;
            
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if(rb != null) rb.velocity = Vector2.zero;

            if (player != null && player.Movement != null)
            {
                player.Movement.SetFacingDirection(facingDirection);
            }
            
            AssignPlayerToCinemachine();
        }
        
        private void AssignPlayerToCinemachine()
        {
            var virtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            
            if (virtualCamera != null && _currentPlayerInstance != null)
            {
                virtualCamera.Follow = _currentPlayerInstance.transform;
            }
        }
        
        public void UnloadCurrentMap()
        {
            if (_currentMapInstance != null)
            {
                _currentMapInstance.SetActive(false); // Important: Hide immediately to prevent FindGameObjectWithTag from finding it
                Destroy(_currentMapInstance);
                _currentMapInstance = null;
            }
            
        }
        
        private void OnGUI()
        {
            if (!showDebugInfo) return;
            
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 16;
            style.normal.textColor = Color.yellow;
            
            string info = $"Level: {_currentLevelIndex}";
            
            GUI.Label(new Rect(10, 10, 300, 60), info, style);
        }
    }
}
