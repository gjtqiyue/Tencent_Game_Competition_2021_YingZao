using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : BaseControlUnit
{
    static DialogueManager Instance;

    public DialogueView view;
    public bool allowSkip = false;

    public delegate void OnDialogueEventFinished();
    public delegate void OnScenarioEventStart();
    public delegate void OnScenarioEventEnd();

    public event OnDialogueEventFinished dialogueEventFinishedDelegate;
    public event OnScenarioEventStart scenarioEventStartDelegate;
    public event OnScenarioEventEnd scenarioEventEndDelegate;

    private Scenario currentScenario;
    private Scenario prevScenario;
    private Scenario nextScenario;
    private Scenario globalScenario;

    private Dictionary<string, Scenario> scenarioCollection = new Dictionary<string, Scenario>();

    XMLParser dialogueParser = new XMLParser();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Object.Destroy(this.gameObject);
        }
    }

    public static DialogueManager GetInstance() { return Instance; }

    private void Start()
    {
        ParsePlotScript();


        TriggerScenario("Intro");
    }

    public void AddScenario(Scenario s)
    {
        scenarioCollection.Add(s.name, s);
    }

    private void ParsePlotScript()
    {
        string dialogueFilePath = Path.Combine(Application.dataPath, "PlotScript/Script.xml");
        dialogueParser.ParseDialogueScript(this, dialogueFilePath);
    }

    public void TriggerScenario(string name)
    {
        if (!scenarioCollection.ContainsKey(name)) throw new System.Exception("No matching scenario found!");

        //if there is currently something playing, and no global scenario, then register it to the global scenario
        //otherwise give up
        if (currentScenario != null && !currentScenario.Finished())
        {
            if (globalScenario == null) globalScenario = scenarioCollection[name];
            else return;
        }
        //global scenario has higher priority
        if (globalScenario != null)
        {
            //if there is a global, play global and set the input to the next
            currentScenario = globalScenario;
            globalScenario = null;
            nextScenario = scenarioCollection[name];
        }
        else if (nextScenario != null)
        {
            //
            currentScenario = nextScenario;
            nextScenario = scenarioCollection[name];
        }
        else
        {
            currentScenario = scenarioCollection[name];
        }

        //start a coroutine to keep triggering the next event
        //disable input for player other than dialogue interaction
        StartCoroutine(ScenarioCoroutine());
    }

    IEnumerator ScenarioCoroutine()
    {
        while (!currentScenario.Finished())
        {
            currentScenario.Play();
            yield return new WaitUntil(currentScenario.IsDonePlayingCurrentActivity);
        }

        //assign current to prev and trigger next
        prevScenario = currentScenario;
        if (currentScenario.next != null)
        {
            TriggerScenario(currentScenario.next);
        }
        else
        {
            currentScenario = null;
            scenarioEventEndDelegate?.Invoke();
            prevScenario = null;
        }
    }

    public void TriggerFigure(Figure e)
    {
        //determine if it's a NPC or the player

        //tell the view to do the transition
        view.FigureTransition(e.character, e.from, e.to, e.transitionTime);
    }

    public void TriggerQuestion(Question q)
    {
        
    }

    public void TriggerLine(Line l)
    {
        //show dialogue panel
        StartCoroutine(PrintLineToScreen(l.content));
    }
    private bool skipTrigger = true;
    IEnumerator PrintLineToScreen(string text)
    {
        float printSpeed = view.printingSpeed;
        int i = 0;
        while (i < text.Length)
        {
            //TODO: add fade feature using html text 
            while (text[i] == ' ') i++;
            i++;
            view.UpdateLineView(text.Substring(0, i));
            yield return new WaitForSeconds(printSpeed);

            if (allowSkip)
            {
                if (skipTrigger && Keyboard.current.fKey.isPressed)
                {
                    //print the whole string
                    view.UpdateLineView(text);
                    skipTrigger = false;
                    while (true)
                    {
                        if (inputEnabled && !skipTrigger && Keyboard.current.fKey.wasReleasedThisFrame)
                        {
                            skipTrigger = true;
                            i = text.Length + 1;
                            break;
                        }
                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        //wait for user input
        bool nextLine = false;
        while (!nextLine)
        {
            if (inputEnabled && skipTrigger && Keyboard.current.fKey.wasPressedThisFrame) { skipTrigger = false; }
            if (inputEnabled && !skipTrigger && Keyboard.current.fKey.wasReleasedThisFrame) { nextLine = true; skipTrigger = true; }
            yield return new WaitForEndOfFrame();
        }

        //callback
        OnDialogueEventFinish();
    }

    public void OnDialogueEventFinish()
    {
        dialogueEventFinishedDelegate?.Invoke();
    }

    //TODO: once the player exit the range of dialogue it will stop
    //then the system will remember the last scenario and which line it 
    //if the player choose to trigger the same scenario again, it will first trigger a scenario to complain that the player didn't finish
    //say something like "where was I the last time? Oh yes it was...", then continue with the previous dialogue
}
