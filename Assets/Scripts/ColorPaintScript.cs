using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ColorPaintScript : BaseControlUnit
{

    [SerializeField] List<ColorPaintPiece> allPieces;
    public ColorPaintBrush brush;

    [SerializeField] ColorMixToolEnum currentSelect;

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

        SetUIActive(true);
        while (allPieces.Count > 0)
        {
            //update painter location
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 pos = new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane);
            Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(pos);
            Ray r = Camera.main.ScreenPointToRay(pos);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 1000))
            {
                brush.UpdatePosition(new Vector3(hit.point.x, hit.point.y, 0));
            }
            yield return new WaitForEndOfFrame();
        }

        //when all finished, proceed
        DialogueManager.GetInstance().TriggerScenario("ColorWorkPaintGameExit");
        yield return new WaitUntil(DialogueManager.GetInstance().Finished);

        gameObject.SetActive(false);
    }

    public void PaintColor(ColorPaintPiece piece)
    {
        if (piece.paintColor != currentSelect) return;
        piece.PaintColor();
        allPieces.Remove(piece);
    }

    public bool HasColorSelected()
    {
        return currentSelect != ColorMixToolEnum.нч;
    }

    public void SelectTool(string tool)
    {
        //Sprite s = iconList.Get(tool);
        currentSelect = (ColorMixToolEnum)Enum.Parse(typeof(ColorMixToolEnum), tool);
    }
}
