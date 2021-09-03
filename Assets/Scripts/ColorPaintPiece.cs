using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPaintPiece : MonoBehaviour
{
    public ColorMixToolEnum paintColor;

    private void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void PaintColor()
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }
}
