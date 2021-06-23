using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public abstract class DialogueActivity
{
    protected DialogueManager dialogueMgr;
    public bool isPlaying = false;

    public DialogueActivity()
    {
        dialogueMgr = DialogueManager.GetInstance();
    }

    public virtual void Play()
    {
        isPlaying = true;
        dialogueMgr.dialogueEventFinishedDelegate += FinishEvent;
    }

    public virtual void FinishEvent() { isPlaying = false; dialogueMgr.dialogueEventFinishedDelegate -= FinishEvent; }

    public bool IsDonePlayingCurrentActivity() { return !isPlaying; }
}

public class Scenario : DialogueActivity
{
    //list of things to play in the scenario
    //follows order in the xml file
    Queue<DialogueActivity> data = new Queue<DialogueActivity>();
    public string name;
    public string next;

    public Scenario(string n) { name = n; }

    public void AddEvent(DialogueActivity d)
    {
        data.Enqueue(d);
    }

    public override void Play()
    {
        if (data.Count > 0)
        {
            base.Play();
            data.Peek().Play();
            data.Dequeue();
        }
    }

    public bool Finished() { return data.Count == 0; }
}
public class Figure : DialogueActivity
{
    public string character;
    public string screenPosition;
    public Vector2 from;
    public Vector2 to;
    public float transitionTime;
    public string animationToPlay;

    public Figure(string c, string s, Vector2 f, Vector2 t, float time, string a)
    {
        character = c;
        screenPosition = s;
        from = f;
        to = t;
        transitionTime = time;
        animationToPlay = a;
    }
    public override void Play()
    {
        base.Play();
        dialogueMgr.TriggerFigure(this);
    }

    public override string ToString()
    {
        return string.Format("Figure: [character : {0}, position : {1}, from : {2}, to : {3}, time : {4}, anim : {5}]", character, screenPosition, from.ToString(), to.ToString(), transitionTime, animationToPlay);
    }
}

public class Question : DialogueActivity
{
    public override void Play()
    {
        base.Play();
        dialogueMgr.TriggerQuestion(this);
    }
}

public class Line : DialogueActivity
{
    public string name;
    public string content;

    public Line(string n, string c)
    {
        name = n;
        content = c;
    }
    public override void Play()
    {
        base.Play();
        
        dialogueMgr.TriggerLine(this);
    }

    public override string ToString()
    {
        return string.Format("DialogueLine: [character : {0}, line : {1}]", name, content);
    }
}

