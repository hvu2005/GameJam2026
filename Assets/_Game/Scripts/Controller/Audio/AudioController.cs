
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AudioItem
{
    public string name;

    public AudioClip clip;
    [Range(0f, 128f)]
    public float volume;
}


public sealed class AudioController : EventTarget
{

    public static AudioController Instance { get; private set; }
    [SerializeField] private List<AudioItem> audioItems = new();

    private int currentBgIndex = 0;
    private Dictionary<string, AudioSource> _audioItemsDict = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        foreach (var item in audioItems)
        {
            var audioSource = Instantiate(new GameObject("sfx"), transform).AddComponent<AudioSource>();
            audioSource.clip = item.clip;
            audioSource.volume = item.volume;
            _audioItemsDict[item.name] = audioSource;
        }
    }

    void Start()
    {
        // Play("BG_Music_" + currentBgIndex, true);
    }

    public void Play(string name, bool loop = false)
    {
        if (_audioItemsDict.TryGetValue(name, out AudioSource audioSource))
        {
            audioSource.loop = loop;
            audioSource.Play();
        }
    }

    public void Stop(string name)
    {
        if (_audioItemsDict.TryGetValue(name, out AudioSource audioSource))
        {
            audioSource.Stop();
        }
    }
}