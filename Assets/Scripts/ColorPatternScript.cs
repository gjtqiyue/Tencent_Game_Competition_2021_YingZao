using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ColorPatternScript : BaseControlUnit
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] float completedPatternShowPeriod;
    [SerializeField] Vector2 horRange;
    [SerializeField] Vector2 verRange;
    private List<ColorPatternPiece> piecesData;
    private GameObject[] piecePrefabs;

    protected override void Init()
    {
        base.Init();

        piecePrefabs = Resources.LoadAll<GameObject>("Prefabs/PatternPieces");
        piecesData = new List<ColorPatternPiece>();

        //populate pieces array to track what is placed
        for (int j = 0; j < piecePrefabs.Length; j++)
        {
            GameObject piece = Instantiate(piecePrefabs[j], transform);
            piecesData.Add(piece.GetComponent<ColorPatternPiece>());
        }
    }

    private void OnEnable()
    {
        //init all game data
        SetChildrenActive(false);
    }

    private void OnDisable()
    {
        SetChildrenActive(false);
        StopCoroutine(ColorPatternMain());
    }

    private void SetChildrenActive(bool active)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(active);
        }
        
    }

    ColorPatternPiece selectedPiece;
    ColorPatternPiece prev;
    [SerializeField] private float returnToPositionTime;

    public IEnumerator ColorPatternMain()
    {
        //Intro scenario
        DialogueManager.GetInstance().TriggerScenario("ColorWorkPatternGameIntro");
        yield return new WaitUntil(DialogueManager.GetInstance().Finished);

        if (piecesData.Count == 0) yield return 0;
        List<ColorPatternPiece> pieces = piecesData;

        //activate pattern look
        SetChildrenActive(true);
        yield return new WaitForSeconds(completedPatternShowPeriod);

        for (int i = 0; i < pieces.Count; i++)
        {
            //decide a random position
            Vector3 pos = new Vector3(Random.Range(horRange.x, horRange.y), Random.Range(verRange.x, verRange.y), 0);
            //decide a random rotation rotate around z axis
            Vector3 rot = new Vector3(0, 0, Random.Range(0, 360));
            pieces[i].RegisterData(pos, rot);
            pieces[i].ReturnToPosition(returnToPositionTime);
        }
        yield return new WaitForSeconds(1);

        while (pieces.Count > 0)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 pos = new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane);
            Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(pos);
            Ray r = Camera.main.ScreenPointToRay(pos);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 1000))
            {
                if (prev != null) prev.RemoveHighlight();
                if (selectedPiece != null)
                {

                    //if we have something selected
                    if (Mouse.current.leftButton.isPressed)
                    {

                        selectedPiece.transform.position = hit.point;
                    }
                    else
                    {
                        if (selectedPiece.CheckIfValid(hit.point))
                        {
                            selectedPiece.SetToPosition();
                            selectedPiece.RemoveHighlight();
                            pieces.Remove(selectedPiece);
                            selectedPiece = null;
                        }
                        else
                        {
                            //return to position
                            selectedPiece.ReturnToPosition(returnToPositionTime);
                            selectedPiece.RemoveHighlight();
                            selectedPiece = null;
                            //selectedPiece.GetComponent<Collider>().enabled = false;
                        }
                    }
                }
                else
                {
                    if (hit.transform.tag == "ColorPatternPiece" && selectedPiece == null)
                    {
                        //show hover effect
                        ColorPatternPiece hitPiece = hit.transform.GetComponent<ColorPatternPiece>();
                        //hitPiece.ShowHighlight();
                        //cancel previous piece's highlight
                        //if (prev != null)
                        //{
                        //    prev.RemoveHighlight();
                        //}
                        //prev = hitPiece;

                        if (Mouse.current.leftButton.isPressed)
                        {
                            //move with the mouse point
                            selectedPiece = hitPiece;
                            //selectedPiece.GetComponent<Collider>().enabled = true;  //turn the selected object into trigger so the raycast won't hit it
                            hitPiece.RemoveHighlight();
                        }
                    }
                }
            }

            yield return new WaitForEndOfFrame();
        }

        //when all finished, proceed
        DialogueManager.GetInstance().TriggerScenario("ColorWorkPatternGameExit");
        yield return new WaitUntil(DialogueManager.GetInstance().Finished);

        gameObject.SetActive(false);
    }


}
