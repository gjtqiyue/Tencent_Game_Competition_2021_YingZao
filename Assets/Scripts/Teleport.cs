using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour, IInteractable
{
    public void CancelInteraction()
    {
        return;
    }

    public void TriggerInteraction()
    {
        GameManager.GetInstance().ChangeScene();
    }
}
