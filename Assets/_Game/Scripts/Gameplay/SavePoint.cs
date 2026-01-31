using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            SaveManager saveManager = FindObjectOfType<SaveManager>();
            if(saveManager != null)
            {
                saveManager.currentSavePoint = this;
            }
        }
    }
}
