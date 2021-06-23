using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseControlUnit : MonoBehaviour
{
    protected bool inputEnabled = true;
    protected GameManager gameMgr;

    // Start is called before the first frame update
    virtual protected void Start()
    {
        Init();
    }

    /* Initialization when created */
    virtual protected void Init()
    {
        gameMgr = GameManager.GetInstance();

        if (gameMgr != null)
        {
            gameMgr.gameStartLoadingDelegate += DisableInputCheck;
            gameMgr.gameFinishLoadingDelegate += EnableInputCheck;

            gameMgr.gamePauseDelegate += DisableInputCheck;
            gameMgr.gameResumeDelegate += EnableInputCheck;
        }
    }

    protected void DisableInputCheck()
    {
        inputEnabled = false;
    }

    protected void EnableInputCheck()
    {
        inputEnabled = true;
    }
}
