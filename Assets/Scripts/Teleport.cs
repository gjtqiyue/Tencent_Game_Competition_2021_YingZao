using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour, IInteractable
{
    bool inAction = false;

    Dictionary<GameProgressState, bool> state;
    private void Start()
    {
        state = GameManager.GetInstance().GetGameProgress();
    }
    public void CancelInteraction()
    {
        return;
    }

    public void TriggerInteraction()
    {
        if (inAction) return;
        inAction = true;
        //Debug.Log("trigger entrance action");
        state = GameManager.GetInstance().GetGameProgress();
        if (state[GameProgressState.木作] && state[GameProgressState.石作] && state[GameProgressState.漆作])
            StartCoroutine(GoToBuildingScene());
        else
            DialogueManager.GetInstance().TriggerScenario("NotThereYet");

        inAction = false;
    }

    IEnumerator GoToBuildingScene()
    {
        DialogueManager.GetInstance().TriggerScenario("ReadyToGo");
        yield return new WaitUntil(DialogueManager.GetInstance().Finished);
        yield return new WaitForSeconds(0.5f);

        GameManager.GetInstance().ChangeScene();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TriggerInteraction();
    }
}
