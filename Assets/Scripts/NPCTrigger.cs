using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTrigger : MonoBehaviour, IInteractable
{
    public void TriggerInteraction()
    {
        Debug.Log("Trigger NPC conversation");
        DialogueManager.GetInstance().TriggerScenario("Intro");
    }

    public void CancelInteraction()
    {
        return;
    }
}
