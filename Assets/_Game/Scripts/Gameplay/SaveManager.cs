using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // private List<SavePoint> savePoints = new List<SavePoint>();

    [SerializeField] private Transform spawnPoint;
    public SavePoint currentSavePoint;

    private Action<Player> _onPlayerDiedHandler;


    void Awake()
    {
        // SavePoint[] points = FindObjectsOfType<SavePoint>();
        // savePoints.AddRange(points);
    }




    void OnEnable()
    {
        _onPlayerDiedHandler = (data) =>
        {
            if (currentSavePoint == null)
                data.transform.position = spawnPoint.position;
            else
                data.transform.position = currentSavePoint.transform.position;
        };

        EventBus.On<Player>(SceneTransitionType.OnPlayerDied, _onPlayerDiedHandler);
    }

    void OnDisable()
    {
        EventBus.Off<Player>(SceneTransitionType.OnPlayerDied, _onPlayerDiedHandler);
    }

}
