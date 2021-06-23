using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueView : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public float printingSpeed;
    public Image leftCharacterSlot;

    private DialogueManager dialogueMgr;

    // Start is called before the first frame update
    void Start()
    {
        dialogueMgr = DialogueManager.GetInstance();
        dialogueMgr.dialogueEventFinishedDelegate += ClearUp;
    }

    public void FigureTransition(string character, Vector3 from, Vector3 to, float time)
    {
        RectTransform figureTransform = leftCharacterSlot.rectTransform;
        figureTransform.anchoredPosition = from;
        LeanTween.move(figureTransform, to, time).setOnComplete(() => { dialogueMgr.OnDialogueEventFinish(); });
    }

    public void UpdateLineView(string text)
    {
        dialogueText.text = text;
    }

    private void ClearUp()
    {
        dialogueText.text = "";
    }

    
}
