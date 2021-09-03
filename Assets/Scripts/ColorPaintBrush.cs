using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ColorPaintBrush : MonoBehaviour
{
    public ColorPaintScript controller;

    public void UpdatePosition(Vector3 pos)
    {
        transform.position = pos;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "ColorPaintPiece") return;
        Debug.Log("collide with " + collision.GetComponent<ColorPaintPiece>().paintColor);
        if (Mouse.current.leftButton.isPressed && controller.HasColorSelected())
        {
            controller.PaintColor(collision.GetComponent<ColorPaintPiece>());
        }
    }
}
