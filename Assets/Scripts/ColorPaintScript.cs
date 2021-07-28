using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPaintScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        //init all game data
        SetUIActive(false);
    }

    private void OnDisable()
    {
        SetUIActive(false);
        StopCoroutine(ColorPaintMain());
    }

    private void SetUIActive(bool active)
    {
        transform.Find("Canvas").gameObject.SetActive(active);
    }

    public IEnumerator ColorPaintMain()
    {
        //Intro scenario
        DialogueManager.GetInstance().TriggerScenario("ColorWorkPaintGameIntro");
        yield return new WaitUntil(DialogueManager.GetInstance().Finished);

        //when all finished, proceed
        DialogueManager.GetInstance().TriggerScenario("ColorWorkPaintGameExit");
        yield return new WaitUntil(DialogueManager.GetInstance().Finished);

        gameObject.SetActive(false);
    }
}
