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
        
        [Header("Map ID to Load")]
        [SerializeField] private string mapID = "1";
        
        [Header("Player Settings")]
        [Tooltip("Player prefab để spawn khi load map")]
        [SerializeField] private GameObject playerPrefab;
        [Tooltip("Tự động spawn player khi load map")]
        [SerializeField] private bool spawnPlayerOnLoad = true;
        
        [Header("Settings")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private bool autoLoadOnStart = true;
        [SerializeField] private bool showDebugInfo = true;
        
        private GameObject currentMapInstance;
        private GameObject currentPlayerInstance;
        private string currentLoadedMapID = "";
        
        private void Start()
        {
            if (mapDatabase == null)
            {
                Debug.LogError("MapDatabase chưa được gán! Vui lòng gán MapDatabase vào Inspector.");
                return;
            }
            
            if (showDebugInfo)
            {
                Debug.Log("=== Simple Map Loader ===");
                Debug.Log($"Total maps: {mapDatabase.GetTotalMaps()}");
                Debug.Log("Nhập Map ID trong Inspector");
                Debug.Log("Ấn R để load map");
            }
            
            if (autoLoadOnStart)
            {
                LoadMap();
            }
        }
        
        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                LoadMap();
            }
        }
        
        public void LoadMap()
        {
            if (mapDatabase == null)
            {
                Debug.LogError("MapDatabase không được gán!");
                return;
            }
            
            GameObject mapPrefab = mapDatabase.GetMapPrefab(mapID);
            
            if (mapPrefab == null)
            {
                return;
            }
            
            UnloadCurrentMap();
            
            Vector3 position = spawnPoint != null ? spawnPoint.position : Vector3.zero;
            Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;
            
            currentMapInstance = Instantiate(mapPrefab, position, rotation);
            currentMapInstance.name = $"Map_{mapID}";
            currentLoadedMapID = mapID;
            
            if (showDebugInfo)
            {
                Debug.Log($"✓ Loaded Map ID: {mapID}");
            }
            
            if (spawnPlayerOnLoad)
            {
                SpawnPlayerAtSpawnPoint();
            }
        }
        
        private void SpawnPlayerAtSpawnPoint()
        {
            GameObject spawnPointObj = GameObject.FindGameObjectWithTag("PlayerSpawn");
            
            if (spawnPointObj == null)
            {
                if (showDebugInfo)
                    Debug.LogWarning("Không tìm thấy GameObject với tag 'PlayerSpawn' trong map!");
                return;
            }
            
            Vector3 spawnPosition = spawnPointObj.transform.position;
            
            Player player = FindObjectOfType<Player>();
            
            if (player == null)
            {
                if (playerPrefab == null)
                {
                    if (showDebugInfo)
                        Debug.LogWarning("Player prefab chưa được gán! Không spawn player.");
                    return;
                }
                
                currentPlayerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
                currentPlayerInstance.name = "Player";
                player = currentPlayerInstance.GetComponent<Player>();
                
                if (showDebugInfo)
                    Debug.Log($"✓ Created new Player at {spawnPosition}");
            }
            else
            {
                player.transform.position = spawnPosition;
                currentPlayerInstance = player.gameObject;
                
                if (showDebugInfo)
                    Debug.Log($"✓ Teleported Player to {spawnPosition}");
            }
            
            int facingDirection = spawnPointObj.transform.localScale.x >= 0 ? 1 : -1;
            if (player != null && player.Movement != null)
            {
                player.Movement.SetFacingDirection(facingDirection);
            }
            
            AssignPlayerToCinemachine();
        }
        
        private void AssignPlayerToCinemachine()
        {
            var virtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            
            if (virtualCamera != null && currentPlayerInstance != null)
            {
                virtualCamera.Follow = currentPlayerInstance.transform;
            }
        }
        
        public void UnloadCurrentMap()
        {
            if (currentMapInstance != null)
            {
                if (showDebugInfo)
                    Debug.Log($"Unloading Map ID: {currentLoadedMapID}");
                    
                Destroy(currentMapInstance);
                currentMapInstance = null;
                currentLoadedMapID = "";
            }
        }
        
        public void SetMapID(string id)
        {
            mapID = id;
        }
        
        public void SetAndLoadMap(string id)
        {
            mapID = id;
            LoadMap();
        }
        
        private void OnGUI()
        {
            if (!showDebugInfo) return;
            
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 16;
            style.normal.textColor = Color.yellow;
            style.padding = new RectOffset(10, 10, 10, 10);
            
            string info = $"Map ID: {mapID}";
            info += $" (Loaded: {currentLoadedMapID})";
            info += "\nPress R to load";
            
            GUI.Label(new Rect(10, 10, 300, 60), info, style);
        }
    }
}
