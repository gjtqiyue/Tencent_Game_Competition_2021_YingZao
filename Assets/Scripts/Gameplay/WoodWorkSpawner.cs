using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WoodWorkSpawner : BaseControlUnit
{
    [Header("Data")]
    public GameObject woodPrefab;
    public GameObject woodCutPointPrefab;
    public WoodWorkCutter woodCutter;
    public Vector3 spawningPoint;

    [SerializeField]
    int numberOfLumber = 1;
    [SerializeField]
    float transportSpeed = 1;
    [SerializeField]
    float cutPointSpawnWait = 0.2f;
    [SerializeField]
    float visiblePoint = 2;
    [SerializeField]
    float deletePoint;  //relatively to the spawning point on x axis
    [SerializeField]
    float passThreshold = 0.8f;
    [SerializeField]
    float cutLineChangeProbability = 0.2f;
    [SerializeField]
    float cutLineTransitionStep = 0.1f;

    [Header("UI")]
    public TextMeshProUGUI taskUI;
    public TextMeshProUGUI scoreUI;

    private AudioSource sound;
    private GameObject spawnedWood;
    private int lumberCutCount = 0;
    private int cutPointSpawned;
    private float currentCutPoint;

    protected override void Init()
    {
        base.Init();
        sound = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        //init all game data
        SetUIActive(false);
        woodCutter.gameObject.SetActive(false);
        StartCoroutine(WoodWorkMain(numberOfLumber));
        
    }

    private void OnDisable()
    {
        lumberCutCount = 0;
        cutPointSpawned = 0;
        StopCoroutine(WoodWorkMain(numberOfLumber));
    }

    private void SetUIActive(bool active)
    {
        taskUI.gameObject.SetActive(active);
        scoreUI.gameObject.SetActive(active);
    }

    private void FixedUpdate()
    {
        if (spawnedWood != null)
        {
            spawnedWood.transform.position += new Vector3(Time.deltaTime * transportSpeed, 0, 0);
        }
    }

    IEnumerator WoodWorkMain(int numberOfLumber)
    {
        //Intro scenario
        DialogueManager.GetInstance().TriggerScenario("WoodWorkGameIntro");
        yield return new WaitUntil(DialogueManager.GetInstance().Finished);

        yield return new WaitForSeconds(1f);
        SetUIActive(true);
        taskUI.text = "完成度: " + ((float)lumberCutCount / numberOfLumber);

        while (lumberCutCount < numberOfLumber)
        {
            woodCutter.gameObject.SetActive(true);
            spawnedWood = Instantiate(woodPrefab, spawningPoint, Quaternion.identity, gameObject.transform);
            Vector3 upperRightBound = spawnedWood.transform.Find("UpperRightBound").position;
            Vector3 bottomLeftBound = spawnedWood.transform.Find("BottomLeftBound").position;
            float verticalRange = Mathf.Abs(upperRightBound.y - bottomLeftBound.y) / 2;
            float horizontalRange = Mathf.Abs(upperRightBound.x - bottomLeftBound.x);

            // wait until it passes the visible point
            // then we start to spawn cut point for user to cut
            // until it reaches the deletion point
            bool inTransition = false;
            float transitionStep = 0;
            float nextPoint = 0;
            woodCutter.BeginWoodCut();
            while (spawnedWood.transform.position.x - spawningPoint.x < deletePoint)
            {
                float distance = spawnedWood.transform.position.x - spawningPoint.x;
                if (distance > visiblePoint && (distance - visiblePoint) < horizontalRange)
                {
                    //start spawning a cut point at visible point
                    float yPos = 0;
                    if (distance - visiblePoint < Mathf.Epsilon) 
                        currentCutPoint = Random.Range(-verticalRange, verticalRange);
                    if (Random.Range(0f, 1f) < cutLineChangeProbability && !inTransition)
                    {
                        //change plot, sample a new point
                        inTransition = true;
                        nextPoint = Random.Range(-verticalRange, verticalRange);
                    }
                    if (inTransition)
                    {
                        yPos = Mathf.Lerp(currentCutPoint, nextPoint, transitionStep);
                        transitionStep += cutLineTransitionStep;
                        if (transitionStep > 1)
                        {
                            inTransition = false;
                            currentCutPoint = nextPoint;
                            transitionStep = 0;
                        }
                    }
                    else
                    {
                        yPos = currentCutPoint;
                    }

                    //the pivot point is at the right middle point of the wood
                    Instantiate(woodCutPointPrefab, new Vector3(spawnedWood.transform.position.x - Mathf.Clamp(distance - visiblePoint, spawningPoint.y, horizontalRange), yPos + spawningPoint.y, 0), Quaternion.identity, spawnedWood.transform);
                    cutPointSpawned += 1;

                    yield return new WaitForSeconds(cutPointSpawnWait);
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            Destroy(spawnedWood);
            float pass = woodCutter.FinishWoodCut(cutPointSpawned);
            if (pass > passThreshold)
            {
                lumberCutCount += 1;//this value should be monitored by the UI
            }
            yield return new WaitForSeconds(2);
            cutPointSpawned = 0;
        }

        SetUIActive(false);

        //Exit scenario
        DialogueManager.GetInstance().TriggerScenario("WoodWorkGameExit");
        yield return new WaitUntil(DialogueManager.GetInstance().Finished);

        //Hide panel
        gameObject.SetActive(false);
        gameMgr.FinishMiniWork(GameProgressState.木作, true);
    }

    private void Update()
    {
        //update score
        float acc = ((float)woodCutter.GetCurrentCount() / cutPointSpawned) * 100;
        scoreUI.text = "准确度: " + (float.IsNaN(acc)?"0.00": acc.ToString("F2")) + "%";
        taskUI.text = "完成度: " + ((float)lumberCutCount + " / " + numberOfLumber);
    }
}
