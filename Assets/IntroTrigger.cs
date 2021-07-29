using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntroTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] TextMeshProUGUI hintText;

    public void CancelInteraction()
    {
        return;
    }

    public void TriggerInteraction()
    {
        DialogueManager.GetInstance().TriggerScenario("Intro");
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
