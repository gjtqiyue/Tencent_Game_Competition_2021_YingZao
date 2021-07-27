using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanel : MonoBehaviour
{
    float endX;
    [SerializeField]
    float fadeDistance;
    [SerializeField]
    float fadeSpeed;

    public TextMeshProUGUI text;
    public Image spriteRenderer;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        endX = rectTransform.position.x;
        rectTransform.position = new Vector3(endX + fadeDistance, transform.position.y, transform.position.z);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        rectTransform.position = new Vector3(endX + fadeDistance, transform.position.y, transform.position.z);
        LeanTween.move(this.gameObject, new Vector3(endX, rectTransform.position.y, rectTransform.position.z), fadeSpeed);
    }

    public void Disable()
    {
        Vector3 pos = new Vector3(endX + fadeDistance, transform.position.y, transform.position.z);
        LeanTween.move(this.gameObject, pos, fadeSpeed).setOnComplete(()=> { gameObject.SetActive(false); });
    }

    public void UpdateInfo(string info, string img)
    {
        Sprite s = Resources.Load<Sprite>("Resources/Info/" + img);
        if (s != null)
            spriteRenderer.sprite = s;
        
        text.text = info;
    }
}
