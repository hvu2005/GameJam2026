using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // private List<SavePoint> savePoints = new List<SavePoint>();

    [SerializeField] private Transform spawnPoint;
    public SavePoint currentSavePoint;

    void Awake()
    {
        // SavePoint[] points = FindObjectsOfType<SavePoint>();
        // savePoints.AddRange(points);
    }


    void Start()
    {
        EventBus.On<Player>(SceneTransitionType.OnPlayerDied, data =>
        {
            if (currentSavePoint == null) data.transform.position = spawnPoint.position;
            else
                data.transform.position = currentSavePoint.transform.position;
        });
    }
}
