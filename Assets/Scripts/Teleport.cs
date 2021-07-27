using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour, IInteractable
{
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
        state = GameManager.GetInstance().GetGameProgress();
        if (state[GameProgressState.木作] && state[GameProgressState.石作] && state[GameProgressState.漆作])
            GameManager.GetInstance().ChangeScene();
        else
            DialogueManager.GetInstance().TriggerScenario("NotThereYet");
    }
}
