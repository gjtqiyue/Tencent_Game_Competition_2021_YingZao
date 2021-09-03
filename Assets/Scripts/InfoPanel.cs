using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanel : MonoBehaviour
{
    float endY;
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
        endY = rectTransform.position.y;
        rectTransform.position = new Vector3(transform.position.x, endY + fadeDistance, transform.position.z);
    }

    private void OnEnable()
    {
        rectTransform.position = new Vector3(transform.position.x, endY + fadeDistance, transform.position.z);
        LeanTween.move(this.gameObject, new Vector3(transform.position.x, endY, rectTransform.position.z), fadeSpeed);
    }

    public void Disable()
    {
        Vector3 pos = new Vector3(transform.position.x, endY + fadeDistance, transform.position.z);
        LeanTween.move(this.gameObject, pos, fadeSpeed).setOnComplete(()=> { gameObject.SetActive(false); });
    }

    public void UpdateInfo(string info, string img)
    {
        Sprite s = Resources.Load<Sprite>("Info/" + img);
        spriteRenderer.sprite = null;
        if (s != null)
            spriteRenderer.sprite = s;
        
        text.text = info;
    }
}
