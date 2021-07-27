using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using UnityEngine;

public class XMLParser
{
    public void ParseDialogueScript(DialogueManager dialogueManager, string path)
    {
        string text = Resources.Load<TextAsset>("PlotScript/Script").text;
        XElement xmlDoc = XElement.Parse(text);
        foreach (XElement scenario in xmlDoc.Elements("Scenario"))
        {
            Scenario ss = new Scenario(scenario.Attribute("name").Value, scenario.Attribute("isLoop").Value=="True");
            foreach (XElement activity in scenario.Elements())
            {
               switch (activity.Name.ToString())
                {
                    case "Figure":
                        Vector2 from = new Vector2(), to = new Vector2();
                        float time = 0;
                        XAttribute target = activity.Attribute("target");
                        if (target == null) throw new System.Exception("No target specified for Figure");
                        XAttribute name = activity.Attribute("type");
                        XElement side = activity.Element("ScreenPosition");
                        XElement transform = activity.Element("Transform");
                        XElement image = activity.Element("Image");
                        if (transform != null)
                        {
                            XElement fromE = transform.Element("From");
                            XElement toE = transform.Element("To");
                            XElement timeE = transform.Element("Time");
                            if (fromE != null) from = ConvertToVector2(transform.Element("From").Value);
                            if (toE != null) to = ConvertToVector2(transform.Element("To").Value);
                            if (timeE != null) time = float.Parse(transform.Element("Time").Value);
                        }

                        Figure f = new Figure(
                            target.Value,
                            side == null ? "left" : side.Value,
                            from, to, time,
                            image == null ? "" : image.Attribute("name").Value
                        );
                        //Debug.Log(f.ToString());
                        ss.AddEvent(f);
                        break;
                    case "Line":
                        ss.AddEvent(new Line(activity.Attribute("name").Value, activity.Value));
                        break;
                    case "Question":
                        break;
                    case "GoTo":
                        ss.next = activity.Value;
                        break;
                    default:
                        throw new System.Exception("Unrecognizable element in plot script");
                }
            }
            dialogueManager.AddScenario(ss);
        }
    }

    Vector2 ConvertToVector2(string value)
    {
        string[] array = value.Split(',');
        return new Vector2(float.Parse(array[0].Trim()), float.Parse(array[1].Trim()));
    }

    /* We can use XML to save and load game data */
    //void LoadSaveData()
    //{
    //    if (File.Exists(filePath))
    //    {
    //        string fileText = File.ReadAllText(filePath);
    //        XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
    //        using (StringReader reader = new StringReader(fileText))
    //        {
    //            return (PlayerData)(serializer.Deserialize(reader)) as PlayerData;
    //        }
    //    }
    //}

    //void SaveData(PlayerData data)
    //{
    //    var serializer = new XmlSerializer(typeof(PlayerData));

    //    using (var stream = new FileStream(filePath, FileMode.Create))
    //    {
    //        serializer.Serialize(stream, data);
    //    }
    //}

}
