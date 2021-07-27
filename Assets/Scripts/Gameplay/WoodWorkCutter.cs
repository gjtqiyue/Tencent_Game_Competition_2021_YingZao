using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WoodWorkCutter : BaseControlUnit
{
    [SerializeField]
    private float minH = 1;
    [SerializeField]
    private float maxH = 1;
    [SerializeField]
    private float moveSpeed = 1;
    private float vertical;
    private int currentCount = 0;



    // Update is called once per frame
    void Update()
    {
        //Input check
        vertical = 0;
        if (!inputEnabled) return;

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed)
                vertical = 1;
            if (keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed)
                vertical = -1;
        }
    }

    private void FixedUpdate()
    {
        Vector3 pos = gameObject.transform.position;
        pos += new Vector3(0, vertical * Time.deltaTime * moveSpeed, 0);

        pos.y = Mathf.Clamp(pos.y, minH, maxH);
        transform.position = pos;
    }

    public void BeginWoodCut()
    {
        currentCount = 0;
    }

    //return percentage on how many points are cut successfully
    public float FinishWoodCut(int goal)
    {
        return (float)currentCount / (float)goal;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "WoodCutPoint") return;

        currentCount += 1;
        Destroy(other.gameObject);
    }

    public int GetCurrentCount() { return currentCount; }
}
