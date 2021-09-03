using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject entrancePanel;
    public GameObject menuContent;
    [SerializeField] bool inMenu = false;

    // Start is called before the first frame update
    void Start()
    {
        menuContent.SetActive(false);
        entrancePanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!inMenu)
        {
            if (Keyboard.current.anyKey.IsPressed() || Mouse.current.leftButton.IsPressed())
            {
                //Debug.Log("enter menu");
                inMenu = true;
                EnterMenu();
            }
        }
    }

    void EnterMenu()
    {
        entrancePanel.SetActive(false);
        menuContent.SetActive(true);
    }
}
