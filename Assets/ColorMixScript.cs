using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;



public class ColorMixScript : BaseControlUnit
{
    public struct ColorMixRecipe
    {
        public List<string> colorOrder;
        public int waterRatio;
        public int glueRatio;
        public List<ColorMixToolEnum> recipe;

        public ColorMixRecipe(List<string> color, int water, int glue)
        {
            recipe = new List<ColorMixToolEnum>();
            colorOrder = color;
            waterRatio = water;
            glueRatio = glue;

            for (int i = 0; i < colorOrder.Count; i++)
            {
                recipe.Add((ColorMixToolEnum)Enum.Parse(typeof(ColorMixToolEnum), colorOrder[i]));
                for (int j = 0; j < waterRatio; j++)
                {
                    recipe.Add(ColorMixToolEnum.水);
                }
                for (int j = 0; j < glueRatio; j++)
                {
                    recipe.Add(ColorMixToolEnum.骨胶);
                }
            }
        }
    }

    public TextMeshProUGUI stepRemainUI;
    List<ColorMixRecipe> receipes;
    [SerializeField] List<ColorMixToolEnum> currentMixing;
    ColorMixToolEnum currentSelect;

    protected override void Init()
    {
        base.Init();

        receipes = new List<ColorMixRecipe>();
        //ColorMixRecipe r1 = new ColorMixRecipe(new List<string> { "白" }, 2, 1);
        //ColorMixRecipe r2 = new ColorMixRecipe(new List<string> { "青" }, 1, 3);
        //receipes.Add(r1);
        //receipes.Add(r2);
    }

    private void OnEnable()
    {
        //init all game data
        SetUIActive(false);
    }

    private void OnDisable()
    {
        SetUIActive(false);
        StopCoroutine(ColorMixMain());
    }

    private void SetUIActive(bool active)
    {
        transform.Find("Canvas").gameObject.SetActive(active);
    }

    public IEnumerator ColorMixMain()
    {
        //Intro scenario
        DialogueManager.GetInstance().TriggerScenario("ColorWorkMixGameIntro");
        yield return new WaitUntil(DialogueManager.GetInstance().Finished);

        yield return new WaitForSeconds(1f);
        SetUIActive(true);

        int idx = 0;
        while (idx < receipes.Count)
        {
            ColorMixRecipe recipe = receipes[idx];
            yield return PreMixingStep(recipe);
            yield return MixingStep(recipe);
            yield return PostMixingStep(recipe);
            idx++;
        }
        yield return new WaitForEndOfFrame();

        //Exit scenario
        DialogueManager.GetInstance().TriggerScenario("ColorWorkMixGameExit");
        yield return new WaitUntil(DialogueManager.GetInstance().Finished);

        gameObject.SetActive(false);
    }

    IEnumerator PreMixingStep(ColorMixRecipe r)
    {
        yield return null;
    }

    IEnumerator MixingStep(ColorMixRecipe r)
    {

        int stepTotal = r.recipe.Count;
        while (true)
        {
            currentMixing = new List<ColorMixToolEnum>();
            //wait until no more steps are needed
            while (currentMixing.Count < stepTotal)
            {
                //update UI
                stepRemainUI.text = "剩余步骤 " + (stepTotal - currentMixing.Count) + " / " + stepTotal;
                yield return new WaitForEndOfFrame();
            }

            //check if results are correct
            if (FinishCheck(r.recipe))
            {
                break;
            }
            else
            {
                Debug.Log("failed");
                DialogueManager.GetInstance().TriggerScenario("FailedAtColorMix");
                yield return new WaitUntil(DialogueManager.GetInstance().Finished);
                
            }
        }


        yield return new WaitForEndOfFrame();
    }

    IEnumerator PostMixingStep(ColorMixRecipe r)
    {
        yield return new WaitForEndOfFrame();
        //play the succeed sound
    }



    public void ResetMix()
    {
        Debug.Log("reset mixing");
        currentMixing.Clear();
        //reset color
    }

    public bool FinishCheck(List<ColorMixToolEnum> r)
    {
        //compare mixing with the receipe
        for (int i=0; i<r.Count; i++)
        {
            if (r[i] != currentMixing[i]) return false;
        }
        return true;
    }

    public void AddSelectedMixture()
    {
        if (currentSelect == ColorMixToolEnum.无) return;

        currentMixing.Add(currentSelect);

        //trigger post effect
    }

    
    public void SelectTool(string tool)
    {
        Debug.Log(tool);
        currentSelect = (ColorMixToolEnum)Enum.Parse(typeof(ColorMixToolEnum), tool);
    }
}
