using UnityEngine;

namespace _Game.Scripts._Config.Data
{
    [CreateAssetMenu(fileName = "MapDatabase", menuName = "Game/Map Database", order = 0)]
    public class MapDatabase : ScriptableObject
    {
        [Header("Map Prefabs")]
        [Tooltip("Kéo map prefabs vào theo thứ tự: Element 0 = Map ID 1, Element 1 = Map ID 2...")]
        public GameObject[] mapPrefabs;
        
        public GameObject GetMapPrefab(string mapId)
        {

            foreach(GameObject i in mapPrefabs)
            {
                if(i.name == mapId)
                {
                    return i;
                }
            }
            
            return null;
        }
        
        public int GetTotalMaps() => mapPrefabs.Length;
    }
}
