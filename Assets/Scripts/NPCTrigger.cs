using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] string scenarioBeforeCompletion;
    [SerializeField] string scenarioAfterCompletion;
    [SerializeField] GameProgressState workType;
    public void TriggerInteraction()
    {
        StartCoroutine(TriggerNPCTalk());
    }

    IEnumerator TriggerNPCTalk()
    {
        if (!GameManager.GetInstance().GetGameProgress()[workType])
        {
            DialogueManager.GetInstance().TriggerScenario(scenarioBeforeCompletion);
            yield return new WaitUntil(DialogueManager.GetInstance().Finished);
            GameManager.GetInstance().PlayMiniWorkGame(workType);
        }
        else
        {
            DialogueManager.GetInstance().TriggerScenario(scenarioAfterCompletion);
        }
    }

    public void CancelInteraction()
    {
        return;
    }
}
