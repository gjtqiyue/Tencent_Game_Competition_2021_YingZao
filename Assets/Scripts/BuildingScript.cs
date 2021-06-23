using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.EventSystems;

public class BuildingScript : BaseControlUnit
{
    public static BuildingScript Instance;

    public BlueprintSelect blueprintSelect;
    public ComponentSelect componentSelect;
    public TextMeshProUGUI blueprintButtonText;
    public GameObject blueprintImage;
    public Material previewMat;

    [SerializeField]
    private List<string> componentNameSet = new List<string>();
    [SerializeField]
    private List<Sprite> componentImageSet = new List<Sprite>();

    [SerializeField]
    private bool isBlueprintOpen = false;
    [SerializeField]
    private BlueprintType currentBlueprint;

    private ConstructionData data;
    private ComponentPrefabTable prefabTable;
    private ConstructionOrder currentStep;
    private List<ComponentTransformInfo> allPossibleSpawnPoints;
    private Dictionary<GameObject, ComponentTransformInfo> previewGameObjects = new Dictionary<GameObject, ComponentTransformInfo>();
    private GameObject spawnedComponentObj;
    private ConsturctionComponent selectedComponent;
    private List<GameObject> spawnedObjects = new List<GameObject>();

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

        data = ConstructionData.Load(Path.Combine(Application.dataPath, "Resources/data.xml"));
        prefabTable = ComponentPrefabTable.Load(Path.Combine(Application.dataPath, "Resources/componentPrefabTable.xml"));
    }

    private void Reset()
    {
        blueprintBuildResetDelegate?.Invoke();
        blueprintSelect.gameObject.SetActive(true);
        currentStep = null;
        foreach (GameObject obj in spawnedObjects)
        {
            Destroy(obj);
        }
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

    public void TriggerBlueprintConstruction(BlueprintType t)
    {
        Debug.Log(t.ToString());
        StartCoroutine(Construct(t));
    }

    public void TriggerBlueprint()
    {
        if (isBlueprintOpen) return;
        ResetSpawnedObjects();
        StartCoroutine(ViewBlueprint());
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



    IEnumerator Construct(BlueprintType t)
    {
        //setup the scene, which will just be a empty plane or empty scene
        Blueprint b = LoadBuildData(t.ToString());

        //TODO: feed corresponding blueprint as input
        yield return StartCoroutine(ViewBlueprint());

        //populate component UI
        componentSelect.PopulateComponentButtons(componentNameSet, componentImageSet);

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
                        spawnedComponentObj = null;
                        
                        //once all compoennts are done, go to the next step
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

        Debug.Log("finish all steps");
        blueprintBuildFinishDelegate?.Invoke();
        Reset();
    }

    private bool CheckIfStepCompleted(ConsturctionComponent component)
    {
        if (allPossibleSpawnPoints.Count == 0)
        {
            Debug.Log("finish current compoenent build");
            currentStep.components.Remove(component);   //remove current component from step list
        }

        if (currentStep.components.Count == 0)
        {
            Debug.Log("finish step");
            return true;
        }

        return false;
    }


    internal void SpawnComponent(string v)
    {
        if (isBlueprintOpen) return;
        Debug.Log("spawn component " + v);
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
                GameObject toSpawn = Resources.Load<GameObject>("Prefabs/" + prefabTable.map[i].prefabName);
                for (int j=0; j<allPossibleSpawnPoints.Count; j++)
                {
                    ComponentTransformInfo info = allPossibleSpawnPoints[j];
                    loc = new Vector3(info.px, info.py, info.pz);
                    rot = Quaternion.Euler(info.rx, info.ry, info.rz);
                    Debug.Log(loc.ToString());
                    GameObject gameObj = Instantiate(toSpawn, loc, rot);
                    //change material to preview material
                    gameObj.GetComponent<MeshRenderer>().material = previewMat;
                    gameObj.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    previewGameObjects.Add(gameObj, info);
                }

                //spawn one for actual compoenent to place
                //find mouse location
                spawnedComponentObj = Instantiate(toSpawn, new Vector3(0,-10,0), Quaternion.identity); //spawn objects off screen
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
