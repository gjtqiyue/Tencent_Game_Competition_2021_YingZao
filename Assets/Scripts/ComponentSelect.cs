using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

[RequireComponent(typeof(CanvasGroup))]
public class ComponentSelect : MonoBehaviour
{
    
    public GameObject componentPrefab;
    public Vector3 orientation;
    public float moveDistance;
    public float moveSpeed = 10;
    public float delayTime = 0.5f;



    // Start is called before the first frame update
    void Start()
    {
        BuildingScript.GetInstance().blueprintBuildResetDelegate += ResetComponentSelectUI;
    }

    void ResetComponentSelectUI()
    {
        Button[] bs = GetComponentsInChildren<Button>();
        for (int i = 0; i < bs.Length; ++i)
        {
            Destroy(bs[i].gameObject);
        }
    }

    public void PopulateComponentButtons(List<string> componentNameSet, List<Sprite> componentImageSet)
    {
        StartCoroutine(PopulateComponentButtonsAnimation(componentNameSet, componentImageSet));
    }

    IEnumerator PopulateComponentButtonsAnimation(List<string> componentNameSet, List<Sprite> componentImageSet)
    {
        if (componentImageSet.Count != componentNameSet.Count) Debug.LogError("component data set size doesn't match!");
        //need a list of button info
        //name, image, type of component, 
        List<GameObject> objs = new List<GameObject>();
        for (int i = 0; i < componentNameSet.Count; i++)
        {
            GameObject gameObj = Instantiate(componentPrefab, gameObject.transform, false);
            Image img = gameObj.GetComponentInChildren<Image>();
            TextMeshProUGUI text = gameObj.GetComponentInChildren<TextMeshProUGUI>();
            CanvasGroup g = gameObj.GetComponent<CanvasGroup>();
            Button b = gameObj.GetComponent<Button>();

            //set up image and text to display
            img.sprite = componentImageSet[i];
            text.text = componentNameSet[i];
            g.alpha = 0;    //make button transparent for now
            //register callback for button click event
            string s = componentNameSet[i];
            b.onClick.AddListener(() => { BuildingScript.GetInstance().SpawnComponent(s); });

            objs.Add(gameObj);
        }

        yield return new WaitForSeconds(0.2f);

        for (int i=0; i<objs.Count; i++)
        {
            RectTransform rect = objs[i].GetComponent<RectTransform>();
            Vector3 dist = rect.position;
            Debug.Log(dist);
            rect.position = dist - orientation * moveDistance;
            //objs[i].SetActive(true);
            objs[i].GetComponent<CanvasGroup>().alpha = 1;
            LeanTween.move(objs[i], dist, moveSpeed).setDelay(delayTime * i);
        }
    }
}
