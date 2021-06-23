using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public void OnBackSceneButtonClick()
    {
        GameManager.GetInstance().ChangeScene();
    }
}
