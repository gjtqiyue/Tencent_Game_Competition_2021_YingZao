using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueView : MonoBehaviour
{
    public GameObject DialoguePanel;
    public TextMeshProUGUI dialogueText;
    public float printingSpeed;
    public Image leftCharacterSlot;
    public Image rightCharacterSlot;

    private DialogueManager dialogueMgr;
    private string leftCharacter;
    private string rightCharacter;

    
    public IconDictionary characterIconMap;

    // Start is called before the first frame update
    void Start()
    {
        dialogueMgr = DialogueManager.GetInstance();
        dialogueMgr.scenarioEventEndDelegate += ClearUp;
        dialogueMgr.scenarioEventStartDelegate += SetUp;
    }

    private void SetUp()
    {
        DialoguePanel.SetActive(true);
        dialogueText.text = "";
        leftCharacterSlot.gameObject.SetActive(false);
        rightCharacterSlot.gameObject.SetActive(false);
    }

    public void FigureTransition(string character, Vector3 from, Vector3 to, float time, string pos, string img)
    {

        if (pos == "left")
        {
            leftCharacterSlot.gameObject.SetActive(true);
            leftCharacter = character;
            leftCharacterSlot.sprite = characterIconMap.Get(img);
            RectTransform figureTransform = leftCharacterSlot.rectTransform;
            figureTransform.anchoredPosition = from;
            LeanTween.move(figureTransform, to, time).setOnComplete(() => { dialogueMgr.OnDialogueEventFinish(); });
        }
        else if (pos == "right")
        {
            rightCharacterSlot.gameObject.SetActive(true);
            rightCharacter = character;
            rightCharacterSlot.sprite = characterIconMap.Get(img);
            RectTransform figureTransform = rightCharacterSlot.rectTransform;
            figureTransform.anchoredPosition = from;
            LeanTween.move(figureTransform, to, time).setOnComplete(() => { dialogueMgr.OnDialogueEventFinish(); });
        }
        else
        {
            Debug.LogError("[DialogueView] No position found for figure");
        }
    }

    public void UpdateLineView(string character, string text)
    {
        if (character == leftCharacter)
        {
            rightCharacterSlot.color = Color.gray;
            leftCharacterSlot.color = Color.white;
        }
        else if (character == rightCharacter)
        {
            rightCharacterSlot.color = Color.white;
            leftCharacterSlot.color = Color.gray;
        }
        dialogueText.text = text;
    }

    private void ClearUp()
    {
        dialogueText.text = "";
        leftCharacter = "";
        rightCharacter = "";
        DialoguePanel.SetActive(false);
        leftCharacterSlot.gameObject.SetActive(false);
        rightCharacterSlot.gameObject.SetActive(false);
    }

    
}
