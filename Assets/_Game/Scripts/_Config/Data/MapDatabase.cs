using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts._Config.Data
{
    [System.Serializable]
    public class LevelData
    {
        public string levelName;
        public GameObject mapPrefab;
        public Sprite backgroundSprite;
    }

    [CreateAssetMenu(fileName = "MapDatabase", menuName = "Game/Map Database", order = 0)]
    public class MapDatabase : ScriptableObject
    {
        [Header("Level Data")]
        [Tooltip("Define list of levels with Map and Background prefabs")]
        public List<LevelData> levels = new List<LevelData>();
        
        public LevelData GetLevelData(int index)
        {
            if (index >= 0 && index < levels.Count)
            {
                return levels[index];
            }
            Debug.LogWarning($"[MapDatabase] Level index {index} out of range! Total levels: {levels.Count}");
            return null;
        }

        public GameObject GetMapPrefab(int index)
        {
            var data = GetLevelData(index);
            return data != null ? data.mapPrefab : null;
        }

        public Sprite GetBackgroundSprite(int index)
        {
            var data = GetLevelData(index);
            return data != null ? data.backgroundSprite : null;
        }
        
        
        public int GetTotalMaps() => levels.Count;
    }
}
