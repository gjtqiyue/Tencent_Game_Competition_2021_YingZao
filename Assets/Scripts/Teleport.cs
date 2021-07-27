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
        if (state[GameProgressState.ľ��] && state[GameProgressState.ʯ��] && state[GameProgressState.����])
            GameManager.GetInstance().ChangeScene();
        else
            DialogueManager.GetInstance().TriggerScenario("NotThereYet");
    }
}
