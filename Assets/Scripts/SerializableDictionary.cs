using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys;

    [SerializeField]
    private List<TValue> values;

    private Dictionary<TKey, TValue> dictionaryData = new Dictionary<TKey, TValue>();

    public SerializableDictionary()
    {
        keys = new List<TKey>();
        values = new List<TValue>();
    }

    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        if (keys.Count == values.Count)
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in dictionaryData)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }
    }

    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        if (keys.Count == 0)
        {
            this.Clear();
        }
        else if (keys.Count == values.Count)
        {     
            dictionaryData.Clear();
            for (int i = 0; i < keys.Count; i++)
                this.Add(keys[i], values[i]);
        }

        
    }

    public void Add(TKey key, TValue data)
    {
        dictionaryData.Add(key, data);
    }

    public void Remove(TKey key)
    {
        values.Remove(dictionaryData[key]);
        keys.Remove(key);
        dictionaryData.Remove(key);

    }

    public bool ContainsKey(TKey key)
    {
        return dictionaryData.ContainsKey(key);
    }

    public bool ContainsValue(TValue data)
    {
        return dictionaryData.ContainsValue(data);
    }

    public void Clear()
    {
        keys.Clear();
        values.Clear();
        dictionaryData.Clear();
    }

    public TValue Get(TKey key) { return dictionaryData[key]; }
}
