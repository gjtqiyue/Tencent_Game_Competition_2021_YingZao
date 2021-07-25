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

    private void Start()
    {
        endX = transform.position.x;
    }

    private void OnEnable()
    {
        transform.position = new Vector3(endX - fadeDistance, transform.position.y, transform.position.z);
        LeanTween.move(this.gameObject, new Vector3(endX, transform.position.y, transform.position.z), fadeSpeed);
    }

    public void Disable()
    {
        LeanTween.move(this.gameObject, new Vector3(endX, transform.position.y, transform.position.z), fadeSpeed).setOnComplete(()=> { gameObject.SetActive(false); });
    }

    public void UpdateInfo(string info, string img)
    {
        Sprite s = Resources.Load<Sprite>("Resources/Info/" + img);
        if (s != null)
            spriteRenderer.sprite = s;
        
        text.text = info;
    }
}
