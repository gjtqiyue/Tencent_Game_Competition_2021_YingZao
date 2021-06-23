using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;

public class PauseMenu : BaseControlUnit
{
    public GameObject panel;
    public Volume blurVolume;
    public float fadeTime = 0.5f;
    public float delay = 0f;
    public float blurDist = 36;

    DepthOfField dof;

    private CanvasGroup canvasGroup;
    protected override void Start()
    {
        base.Start();

        blurVolume.profile.TryGet<DepthOfField>(out dof);

        panel.SetActive(false);
        if (panel.GetComponent<CanvasGroup>())
            canvasGroup = panel.GetComponent<CanvasGroup>();
        else
            Debug.LogError("No CanvasGroup component found on Panel");
        
    }

    public void DisableMenu()
    {
        panel.SetActive(false);
    }

    public void DisplayPauseMenu()
    {
        //aniamte in
        panel.SetActive(true);
        
        canvasGroup.alpha = 0;
        LeanTween.alphaCanvas(canvasGroup, 1, fadeTime).setDelay(delay).setOnComplete(() => { gameMgr.PauseGameCallback(); }) ;
    }

    public void HidePauseMenu()
    {
        //animate out
        LeanTween.alphaCanvas(canvasGroup, 0, fadeTime).setOnComplete(() => { panel.SetActive(false); });
    }
    
}
