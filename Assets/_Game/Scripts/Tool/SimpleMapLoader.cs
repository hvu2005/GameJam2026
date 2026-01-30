using UnityEngine;
using UnityEngine.InputSystem;
using _Game.Scripts._Config.Data;

namespace _Game.Scripts.Tool
{
    public class SimpleMapLoader : MonoBehaviour
    {
        [Header("Map Database")]
        [SerializeField] private MapDatabase mapDatabase;
        
        [Header("Map ID to Load")]
        [SerializeField] private string mapID = "1";
        
        [Header("Settings")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private bool autoLoadOnStart = true;
        [SerializeField] private bool showDebugInfo = true;
        
        private GameObject currentMapInstance;
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
