using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextFlush : MonoBehaviour
{

    private TextMeshProUGUI text;
    private LTDescr loop;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        loop = LeanTween.value(1, 0, 0.5f).setLoopPingPong().setOnUpdate((value) => { text.color = new Color(text.color.r, text.color.g, text.color.b, value); });
    }

    private void OnDisable()
    {
        //Debug.Log("Loop end");
        if (loop != null) loop.setLoopCount(0);
    }
}
