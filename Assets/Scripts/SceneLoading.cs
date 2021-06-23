using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneLoading : MonoBehaviour
{
    public TextMeshProUGUI loadingProgress;
    public GameObject panel;
    public float fadeTime = 0.5f;
    public float delay = 0f;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        panel.SetActive(false);
        if (panel.GetComponent<CanvasGroup>())
            canvasGroup = panel.GetComponent<CanvasGroup>();
        else
            Debug.LogError("No CanvasGroup component found on Panel");

        GameManager.GetInstance().gameStartLoadingDelegate += DisplayLoadingScreen;
        GameManager.GetInstance().gameFinishLoadingDelegate += HideLoadingScreen;
    }

    public void DisplayLoadingScreen()
    {
        //animate in
        panel.SetActive(true);
        canvasGroup.alpha = 0;
        LeanTween.value(gameObject, UpdateAlpha, 0, 1, fadeTime).setDelay(delay);
    }

    public void HideLoadingScreen()
    {
        //animate out
        LeanTween.value(gameObject, UpdateAlpha, 1, 0, fadeTime).setDelay(delay);
    }

    public void UpdateLoadingProgress(float n)
    {
        loadingProgress.text = "Loading ... " + (n * 100).ToString() + "%";
    }

    void UpdateAlpha(float value)
    {
        canvasGroup.alpha = value;
        if (value == 0)
            panel.SetActive(false);
    }


}

