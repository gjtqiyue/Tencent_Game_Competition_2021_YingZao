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
        Debug.Log("trigger entrance action");
        state = GameManager.GetInstance().GetGameProgress();
        if (state[GameProgressState.ľ��] && state[GameProgressState.ʯ��] && state[GameProgressState.����])
            GameManager.GetInstance().ChangeScene();
        else
            DialogueManager.GetInstance().TriggerScenario("NotThereYet");

        inAction = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TriggerInteraction();
    }
}
