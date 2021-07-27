using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneWorkController : BaseControlUnit
{
    private void OnEnable()
    {
        //init all game data
        StartCoroutine(ColorWorkMain());
    }

    private void OnDisable()
    {
        StopCoroutine(ColorWorkMain());
    }

    IEnumerator ColorWorkMain()
    {
        //Intro scenario
        DialogueManager.GetInstance().TriggerScenario("StoneWorkGameIntro");
        yield return new WaitUntil(DialogueManager.GetInstance().Finished);

        //Exit scenario
        DialogueManager.GetInstance().TriggerScenario("StoneWorkGameExit");
        yield return new WaitUntil(DialogueManager.GetInstance().Finished);

        //Hide panel
        gameObject.SetActive(false);
        gameMgr.SetGameState(GameProgressState.Ê¯×÷, true);
    }
}
