using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerBlockEvent
{
    Trigger
}

public class TriggerBlock : EventTarget
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger block.");
            // Add additional logic here for when the player enters the trigger block

            EventBus.Emit(TriggerBlockEvent.Trigger, this);
            this.OnActivate();
        }
    }


    public void OnActivate()
    {
        // Logic to activate the trigger block if needed
    }
}
