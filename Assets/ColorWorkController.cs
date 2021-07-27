using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColorWorkController : BaseControlUnit
{
    ColorWorkState currentState;
    public ColorMixScript colorMix;
    public ColorPatternScript colorPattern;
    public ColorPaintScript colorPaint;

    private void OnEnable()
    {
        //init all game data
        SetChildrenActive(false);
        StartCoroutine(ColorWorkMain());
    }

    private void OnDisable()
    {
        StopCoroutine(ColorWorkMain());
    }

    private void SetChildrenActive(bool active)
    {
        //activate all children
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(active);
        }
    }

    IEnumerator ColorWorkMain()
    {
        yield return new WaitForSeconds(0.5f);
        //Intro scenario
        DialogueManager.GetInstance().TriggerScenario("ColorWorkGameIntro");
        yield return new WaitUntil(DialogueManager.GetInstance().Finished);

        currentState = ColorWorkState.µ÷Æá;
        colorMix.gameObject.SetActive(true);
        yield return StartCoroutine(colorMix.ColorMixMain());

        currentState = ColorWorkState.²Ê»­;
        colorPattern.gameObject.SetActive(true);
        yield return StartCoroutine(colorPattern.ColorPatternMain());

        currentState = ColorWorkState.ÉÏÆá;
        colorPaint.gameObject.SetActive(true);
        yield return StartCoroutine(colorPaint.ColorPaintMain());

        //Exit scenario
        DialogueManager.GetInstance().TriggerScenario("ColorWorkGameExit");
        yield return new WaitUntil(DialogueManager.GetInstance().Finished);
        
        //Hide panel
        gameObject.SetActive(false);
        gameMgr.SetGameState(GameProgressState.Æá×÷, true);
    }
}
