using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] string scenarioBeforeCompletion;
    [SerializeField] string scenarioAfterCompletion;
    [SerializeField] GameProgressState workType;
    [SerializeField] TextMeshProUGUI hintText;
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

    public void ShowInteractableMessage()
    {
        hintText.gameObject.SetActive(true);
        hintText.text = "°´E½»Ì¸";
    }

    public void HideInteractableMessage()
    {
        hintText.gameObject.SetActive(false);
    }

    public void CancelInteraction()
    {
        return;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player") return;
        ShowInteractableMessage();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Player") return;
        HideInteractableMessage();
    }
}
