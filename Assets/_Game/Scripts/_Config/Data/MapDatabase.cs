using UnityEngine;

namespace _Game.Scripts._Config.Data
{
    [CreateAssetMenu(fileName = "MapDatabase", menuName = "Game/Map Database", order = 0)]
    public class MapDatabase : ScriptableObject
    {
        [Header("Map Prefabs")]
        [Tooltip("Kéo map prefabs vào theo thứ tự: Element 0 = Map ID 1, Element 1 = Map ID 2...")]
        public GameObject[] mapPrefabs;
        
        public GameObject GetMapPrefab(int mapId)
        {
            int index = mapId - 1;
            
            if (index < 0 || index >= mapPrefabs.Length)
            {
                Debug.LogError($"Map ID {mapId} không tồn tại! (Có {mapPrefabs.Length} maps)");
                return null;
            }
            
            if (mapPrefabs[index] == null)
            {
                Debug.LogError($"Map ID {mapId} chưa gán prefab!");
                return null;
            }
            
            return mapPrefabs[index];
        }
        
        public int GetTotalMaps() => mapPrefabs.Length;
    }
}
