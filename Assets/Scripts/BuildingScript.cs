using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.Playables;

public class BuildingScript : BaseControlUnit
{
    public static BuildingScript Instance;
    [Header("Settings")]
    [SerializeField]
    private int buildProgress;
    [SerializeField]
    private int buildTotal;
    [SerializeField]
    private bool isBlueprintOpen = false;
    [SerializeField]
    private BlueprintType currentBlueprint;

    [Header("Controled components")]
    public BlueprintSelect blueprintSelect;
    public ComponentSelect componentSelect;
    public TextMeshProUGUI blueprintButtonText;
    public GameObject blueprintImage;
    public GameObject infoPanel;
    public Material previewMat;
    public PlayableDirector finalSequencePlayer;
    public GameObject blueprintButton;
    public Animator cameraSwitch;
    public AudioSource audio;


    [Header("Component data")]
    [SerializeField]
    private List<string> componentNameSet = new List<string>();
    [SerializeField]
    private List<Sprite> componentImageSet = new List<Sprite>();
    [SerializeField]
    private BuildingScenarioDictionary preScenarioMap;
    [SerializeField]
    private BuildingScenarioDictionary postScenarioMap;


    //private data
    private ConstructionData data;
    private ComponentPrefabTable prefabTable;
    private ConstructionOrder currentStep;
    private List<ComponentTransformInfo> allPossibleSpawnPoints;
    private Dictionary<GameObject, ComponentTransformInfo> previewGameObjects = new Dictionary<GameObject, ComponentTransformInfo>();
    private GameObject spawnedComponentObj;
    private ConsturctionComponent selectedComponent;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Transform mainParent;
    private GameObject toSpawn;

    public delegate void OnBlueprintBuildFinish();
    public delegate void OnBlueprintBuildReset();
    public event OnBlueprintBuildFinish blueprintBuildFinishDelegate;
    public event OnBlueprintBuildReset blueprintBuildResetDelegate;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    protected override void Init()
    {
        base.Init();

        blueprintImage.gameObject.SetActive(false);

        mainParent = GameObject.Find("main building").transform;
        data = ConstructionData.Load(Path.Combine(Application.dataPath, "Resources/data.xml"));
        prefabTable = ComponentPrefabTable.Load(Path.Combine(Application.dataPath, "Resources/componentPrefabTable.xml"));
        blueprintButton.SetActive(false);

        buildTotal = Enum.GetValues(typeof(BlueprintType)).Length;
        buildProgress = 0;

        audio.Play();

        //Start main loop
        StartCoroutine(ConstructionMain());
    }

    private void Reset()
    {
        blueprintBuildResetDelegate?.Invoke();
        //blueprintSelect.gameObject.SetActive(true);
        blueprintButton.SetActive(false);
        currentStep = null;
    }

    public static BuildingScript GetInstance() { return Instance; }


    private Blueprint LoadBuildData(string name)
    {
        foreach (Blueprint b in data.blueprints)
        {
            if (b.name == name) return b;
        }
        Debug.LogError("No blueprint found for name " + name);
        return null;
    }

    //public void TriggerBlueprintConstruction(BlueprintType t)
    //{
    //    Debug.Log(t.ToString());
    //    StartCoroutine(Construct(t));
    //}

    public void TriggerBlueprint()
    {
        if (isBlueprintOpen) return;
        ResetSpawnedObjects();
        StartCoroutine(ViewBlueprint());
    }

    IEnumerator ConstructionMain()
    {
        while (buildProgress < buildTotal)
        {
            BlueprintType t = (BlueprintType)Enum.GetValues(typeof(BlueprintType)).GetValue(buildProgress);
            Blueprint b = LoadBuildData(t.ToString());
            //Trigger pre-action
            yield return StartCoroutine(PreConststurction(t, b));

            //Trigger main building process
            yield return StartCoroutine(Construct(b));

            //Trigger after-action
            yield return StartCoroutine(PostConstruction(t));
        }

        //Finish all the build, trigger the camera track
        audio.Stop();
        Debug.Log("finish everything, trigger dolly track");
        finalSequencePlayer.Play();
        while (finalSequencePlayer.state == PlayState.Playing)
        {
            yield return new WaitForEndOfFrame();
        }
        GameManager.GetInstance().FinishGame();
    }

    IEnumerator PreConststurction(BlueprintType t, Blueprint b)
    {
        Debug.Log("Pre process for construction step");
        if (preScenarioMap.Get(t) != "")
        {
            DialogueManager.GetInstance().TriggerScenario(preScenarioMap.Get(t));
            yield return new WaitUntil(DialogueManager.GetInstance().Finished);
        }

        //TODO: feed corresponding blueprint as input
        yield return StartCoroutine(ViewBlueprint());
        blueprintButton.SetActive(true);

        //populate component UI
        string[] list = b.componentList.Split(',');
        List<string> nameSet = new List<string>();
        List<Sprite> imgSet = new List<Sprite>();
        for (int i = 0; i < list.Length; i++)
        {
            int idx = int.Parse(list[i]);
            nameSet.Add(componentNameSet[idx]);
            imgSet.Add(componentImageSet[idx]);
        }
        componentSelect.PopulateComponentButtons(nameSet, imgSet);
        cameraSwitch.Play("BuildingState");
    }

    IEnumerator PostConstruction(BlueprintType t)
    {
        Debug.Log("Post process for construction step");
        cameraSwitch.Play("InitState");
        if (postScenarioMap.Get(t) != "")
        {
            DialogueManager.GetInstance().TriggerScenario(postScenarioMap.Get(t));
            yield return new WaitUntil(DialogueManager.GetInstance().Finished);
        }
    }

    IEnumerator ViewBlueprint()
    {
        isBlueprintOpen = true;
        //show blueprint
        blueprintButtonText.text = "¹Ø±ÕÀ¶Í¼";
        blueprintImage.SetActive(true);
        //wait for user input
        bool confirmed = false;
        while (!confirmed)
        {
            if (inputEnabled && (Keyboard.current.anyKey.isPressed || Mouse.current.leftButton.isPressed)) confirmed = true;
            yield return new WaitForEndOfFrame();
        }
        blueprintImage.SetActive(false);
        blueprintButtonText.text = "´ò¿ªÀ¶Í¼";
        isBlueprintOpen = false;
    }

    IEnumerator Construct(Blueprint b)
    {
        
        //start to give instruction based on order
        int order = 0;
        while (order != b.orders.Count)
        {
            currentStep = b.orders[order];
            Debug.Log("current order " + order);
            //while there is still component left
            while (true)
            {
                bool hasHit = false;
                if (spawnedComponentObj != null)
                {
                    Vector2 mousePos = Mouse.current.position.ReadValue();
                    Vector3 pos = new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane);
                    Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(pos);
                    Ray r = Camera.main.ScreenPointToRay(pos);
                    RaycastHit hit;
                    if (Physics.Raycast(r, out hit, 100))
                    {
                        if (previewGameObjects.ContainsKey(hit.collider.gameObject))
                        {
                            spawnedComponentObj.transform.position = hit.collider.gameObject.transform.position;
                            spawnedComponentObj.transform.rotation = hit.collider.gameObject.transform.rotation;
                            hasHit = true;
                        }
                        else
                        {
                            spawnedComponentObj.transform.position = hit.point;
                            spawnedComponentObj.transform.rotation = Quaternion.identity;
                        }
                    }
                    else
                    {
                        spawnedComponentObj.transform.position = mousePosWorld;
                    }

                    //if click button, then leave the object permanently
                    if (Mouse.current.leftButton.isPressed && hasHit)
                    {
                        allPossibleSpawnPoints.Remove(previewGameObjects[hit.collider.gameObject]);
                        foreach (GameObject key in previewGameObjects.Keys)
                        {
                            Destroy(key);
                        }
                        previewGameObjects.Clear();
                        spawnedObjects.Add(spawnedComponentObj);
                        spawnedComponentObj.GetComponent<Collider>().enabled = false;
                        spawnedComponentObj = null;
                        
                        //once all compoennts are done, go to the next step
                        if (CheckIfStepCompleted(selectedComponent))
                        {
                            selectedComponent = null;
                            order += 1;
                            break;
                        }
                    }
                    else if (Mouse.current.leftButton.isPressed && Keyboard.current.numpadPlusKey.isPressed)
                    {
                        //cheat code, build all at once
                        foreach (ComponentTransformInfo info in allPossibleSpawnPoints)
                        {
                            Vector3 loc = new Vector3(info.px, info.py, info.pz);
                            Quaternion rot = Quaternion.Euler(info.rx, info.ry, info.rz);
                            Debug.Log(loc.ToString());
                            GameObject gameObj = Instantiate(toSpawn, mainParent.position + loc * mainParent.localScale.x, rot, mainParent);
                            gameObj.GetComponent<Collider>().enabled = false;
                        }
                        allPossibleSpawnPoints.Clear();
                        foreach (GameObject key in previewGameObjects.Keys)
                        {
                            Destroy(key);
                        }
                        previewGameObjects.Clear();
                        spawnedObjects.Add(spawnedComponentObj);
                        
                        if (spawnedComponentObj != null) Destroy(spawnedComponentObj);
                        spawnedComponentObj = null;

                        if (CheckIfStepCompleted(selectedComponent))
                        {
                            selectedComponent = null;
                            order += 1;
                            break;
                        }
                    }
                    else if (Mouse.current.rightButton.isPressed)
                    {
                        ResetSpawnedObjects();
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }

        // If the info panel is still active we need to wait until the player closes it and then we can go to the next stage
        while (infoPanel.activeSelf) { yield return new WaitForEndOfFrame(); }

        Debug.Log("finish all steps");
        buildProgress++;
        blueprintBuildFinishDelegate?.Invoke();
        Reset();    //reset and go to next stage
    }

    void ShowInfoPanel(string info, string img)
    {
        if (infoPanel.activeSelf) return;
        if (info == "") return;
        infoPanel.SetActive(true);
        infoPanel.GetComponent<InfoPanel>().UpdateInfo(info, img);
    }

    private bool CheckIfStepCompleted(ConsturctionComponent component)
    {
        if (allPossibleSpawnPoints.Count == 0)
        {
            Debug.Log("finish current compoenent build");
            ShowInfoPanel(component.info, component.img);

            currentStep.components.Remove(component);   //remove current component from step list
        }

        if (currentStep.components.Count == 0)
        {
            Debug.Log("finish step");
            return true;
        }

        return false;
    }

    private void CheckIfBuildingCompleted()
    {
        if (buildProgress >= buildTotal)
        {
            //Finish all the build, trigger the camera track
            Debug.Log("finish everything, trigger dolly track");
            Camera.current.enabled = false;
            finalSequencePlayer.Play();
        }
    }

    internal void SpawnComponent(string v)
    {
        if (isBlueprintOpen) return;
        ResetSpawnedObjects();

        string name = "";
        Vector3 loc = new Vector3();
        Quaternion rot = new Quaternion();



        //find information about the component based on the blueprint
        if (currentStep != null)
        {
            foreach (ConsturctionComponent comp in currentStep.components)
            {
                if (comp.name == v)
                {
                    selectedComponent = comp;   //save current selected component reference
                    allPossibleSpawnPoints = comp.transformInfo;    //get a reference to all the spawning locations
                    name = comp.name;
                    break;
                }
            }
        }

        //for now assume name will be valid
        //find the prefab matching the component name
        for (int i = 0; i < prefabTable.map.Count; i++)
        {
            if (prefabTable.map[i].name == name)
            {
                toSpawn = Resources.Load<GameObject>("Prefabs/3D/" + prefabTable.map[i].prefabName);
                if (toSpawn == null) Debug.LogError("No prefab match name " + name);
                for (int j=0; j<allPossibleSpawnPoints.Count; j++)
                {
                    ComponentTransformInfo info = allPossibleSpawnPoints[j];
                    loc = new Vector3(info.px, info.py, info.pz);
                    rot = Quaternion.Euler(info.rx, info.ry, info.rz);
                    Debug.Log(loc.ToString());
                    GameObject gameObj = Instantiate(toSpawn, mainParent.position + loc * mainParent.localScale.x, rot, mainParent);
                    //change material to preview material
                    if (gameObj.GetComponent<MeshRenderer>())
                    {
                        Material[] intMaterials = new Material[gameObj.GetComponent<MeshRenderer>().materials.Length];
                        for (int t = 0; t < intMaterials.Length; t++)
                        {
                            intMaterials[t] = previewMat;
                        }
                        gameObj.GetComponent<MeshRenderer>().materials = intMaterials;
                        gameObj.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                    else
                    {
                        MeshRenderer[] meshRenderers = gameObj.GetComponentsInChildren<MeshRenderer>();
                        for (int u=0; u<meshRenderers.Length; u++)
                        {
                            Material[] intMaterials = new Material[meshRenderers[u].materials.Length];
                            for (int t = 0; t < intMaterials.Length; t++)
                            {
                                intMaterials[t] = previewMat;
                            }
                            meshRenderers[u].materials = intMaterials;
                            meshRenderers[u].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        }
                    }
                    previewGameObjects.Add(gameObj, info);
                }
                

                //spawn one for actual compoenent to place
                //find mouse location
                spawnedComponentObj = Instantiate(toSpawn, new Vector3(0,-10,0), Quaternion.identity, mainParent); //spawn objects off screen
                spawnedComponentObj.gameObject.layer = 2; //Ignore ray cast for pre-spawn object
            }
        }

    }

    private void ResetSpawnedObjects()
    {
        if (previewGameObjects.Count > 0)
        {
            foreach (GameObject key in previewGameObjects.Keys)
            {
                Destroy(key);
            }
        }
        previewGameObjects.Clear();
        if (spawnedComponentObj != null) Destroy(spawnedComponentObj);
    }

}
