using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArchViewControl : BaseControlUnit
{
    [Range(0, 1)]
    public float rotSensitivityX = 1f;
    [Range(0, 1)]
    public float rotSensitivityY= 1f;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        CinemachineCore.GetInputAxis = this.HandleAxisInputDelegate;
    }

    private float HandleAxisInputDelegate(string axisName)
    {
        switch (axisName)
        {
            case "Mouse X":
                //if (Input.touchCount > 0)
                //{
                //    //Is mobile touch
                //    return Input.touches[0].deltaPosition.x / TouchSensitivity_x;
                //}
                if (inputEnabled && Input.GetMouseButton(1))
                {
                    // is mouse click
                    return Input.GetAxis("Mouse X") * rotSensitivityX;
                }
                break;
            case "Mouse Y":
                //if (Input.touchCount > 0)
                //{
                //    //Is mobile touch
                //    return Input.touches[0].deltaPosition.y / TouchSensitivity_y;
                //}
                if (inputEnabled && Input.GetMouseButton(1))
                {
                    // is mouse click
                    return Input.GetAxis(axisName) * rotSensitivityY;
                }
                break;
            default:
                Debug.LogError("Input <" + axisName + "> not recognized.", this);
                break;
        }

        return 0f;
    }

    //TODO: add zoom
}
