using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintSelect : BaseControlUnit
{
    public float fadeTime = 0.5f;

    private CanvasGroup cg;
    protected override void Init()
    {
        base.Init();

        cg = GetComponent<CanvasGroup>();
        FadeIn();
    }

    private void OnEnable()
    {
        FadeIn();
    }

    private void OnDisable()
    {
        
    }

    public void OnBlueprintSelect(string t)
    {
        //trigger building process
        BlueprintType b = (BlueprintType)Enum.Parse(typeof(BlueprintType), t);
        BuildingScript.GetInstance().TriggerBlueprintConstruction(b);

        //fade animation
        FadeOut();
    }

    private void FadeIn()
    {
        LeanTween.value(0, 1, 0.1f).setOnUpdate((value)=> { cg.alpha = value; });
    }

    private void FadeOut()
    {
        LeanTween.value(1, 0, fadeTime).setOnUpdate((value) => { cg.alpha = value; }).setOnComplete(()=> { gameObject.SetActive(false); });
    }
}
