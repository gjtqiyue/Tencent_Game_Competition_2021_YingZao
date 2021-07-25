using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodWorkSpawner : BaseControlUnit
{
    public GameObject woodPrefab;
    public GameObject woodCutPointPrefab;
    public WoodWorkController woodCutter;
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

    private GameObject spawnedWood;
    private int lumberCutCount = 0;
    private int cutPointSpawned;
    private float currentCutPoint;

    private void Start()
    {
        StartWoodWork(numberOfLumber);
    }

    private void FixedUpdate()
    {
        if (spawnedWood != null)
        {
            spawnedWood.transform.position += new Vector3(Time.deltaTime * transportSpeed, 0, 0);
        }
    }

    public void StartWoodWork(int numberOfLumber)
    {
        StartCoroutine(WoodCutting(numberOfLumber));
    }

    IEnumerator WoodCutting(int numberOfLumber)
    {
        while (lumberCutCount < numberOfLumber)
        {
            spawnedWood = Instantiate(woodPrefab, spawningPoint, Quaternion.identity, gameObject.transform);
            Vector3 upperRightBound = spawnedWood.transform.Find("UpperRightBound").position;
            Vector3 bottomLeftBound = spawnedWood.transform.Find("BottomLeftBound").position;
            float verticalRange = (upperRightBound.y - bottomLeftBound.y) / 2;
            float horizontalRange = upperRightBound.x - bottomLeftBound.x;

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
                    if (distance - visiblePoint < Mathf.Epsilon) currentCutPoint = Random.Range(-verticalRange, verticalRange);
                    if (Random.Range(0f, 1f) < cutLineChangeProbability && !inTransition)
                    {
                        //change plot
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
                    Instantiate(woodCutPointPrefab, new Vector3(spawnedWood.transform.position.x - Mathf.Clamp(distance - visiblePoint, 0, horizontalRange), yPos, 0), Quaternion.identity, spawnedWood.transform);
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
            Debug.Log("goal is" + cutPointSpawned + " , pass grade is " + pass);
            if (pass > passThreshold)
            {
                lumberCutCount += 1;//this value should be monitored by the UI
            }
            cutPointSpawned = 0;
        }
    }
}
