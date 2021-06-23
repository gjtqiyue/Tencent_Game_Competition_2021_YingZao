using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

public class Blueprint
{
    [XmlAttribute("Name")]
    public string name;

    [XmlArray("ConstructionOrders")]
    [XmlArrayItem("ConstructionOrder")]
    public List<ConstructionOrder> orders = new List<ConstructionOrder>();

    public override string ToString()
    {
        foreach (ConstructionOrder o in orders)
        {
            Debug.Log(o.ToString());
        }
        return "";
    }
}

public class ConstructionOrder
{
    [XmlAttribute("Name")]
    public string name;

    [XmlArray("Components")]
    [XmlArrayItem("Component")]
    public List<ConsturctionComponent> components = new List<ConsturctionComponent>();
}

public class ConsturctionComponent
{
    [XmlAttribute("Name")]
    public string name;

    [XmlAttribute("Info")]
    public string info;

    [XmlArray("TransformInfo")]
    [XmlArrayItem("ComponentTransformInfo")]
    public List<ComponentTransformInfo> transformInfo = new List<ComponentTransformInfo>();
}

public class ComponentTransformInfo
{
    [XmlElement("PX")]
    public float px;
    [XmlElement("PY")]
    public float py;
    [XmlElement("PZ")]
    public float pz;
    [XmlElement("RX")]
    public float rx;
    [XmlElement("RY")]
    public float ry;
    [XmlElement("RZ")]
    public float rz;
};

[XmlRoot("ConstructionData")]
public class ConstructionData
{
    [XmlArray("Blueprints")]
    [XmlArrayItem("Blueprint")]
    public List<Blueprint> blueprints = new List<Blueprint>();

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(ConstructionData));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    public static ConstructionData Load(string path)
    {
        var serializer = new XmlSerializer(typeof(ConstructionData));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as ConstructionData;
        }
    }

    //Loads the xml directly from the given string. Useful in combination with www.text.
    public static ConstructionData LoadFromText(string text)
    {
        var serializer = new XmlSerializer(typeof(ConstructionData));
        return serializer.Deserialize(new StringReader(text)) as ConstructionData;
    }
}

public class Entry
{
    [XmlAttribute("Name")]
    public string name;

    [XmlElement("Prefab")]
    public string prefabName;
}

[XmlRoot("ComponentPrefabTable")]
public class ComponentPrefabTable
{
    [XmlArray("Entries")]
    [XmlArrayItem("Entry")]
    public List<Entry> map = new List<Entry>();

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(ComponentPrefabTable));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    public static ComponentPrefabTable Load(string path)
    {
        var serializer = new XmlSerializer(typeof(ComponentPrefabTable));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as ComponentPrefabTable;
        }
    }

    //Loads the xml directly from the given string. Useful in combination with www.text.
    public static ComponentPrefabTable LoadFromText(string text)
    {
        var serializer = new XmlSerializer(typeof(ComponentPrefabTable));
        return serializer.Deserialize(new StringReader(text)) as ComponentPrefabTable;
    }
}