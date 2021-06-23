using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public enum GameState
{
    InMenu,
    StartGame,
    InGame,
    FinishGame
};

[System.Serializable]
public class GameManager : MonoBehaviour
{
    private static GameManager Instance;


    /* delegate definition */
    public delegate void OnGamePauseDelegate();
    public delegate void OnGameResumeDelegate();
    public delegate void OnGameStartLoadingDelegate();
    public delegate void OnGameFinishLoadingDelegate();
    public delegate void OnGameStartDelegate();
    public delegate void OnGameEndDelegate();

    public event OnGamePauseDelegate gamePauseDelegate;
    public event OnGamePauseDelegate gameResumeDelegate;
    public event OnGameStartLoadingDelegate gameStartLoadingDelegate;
    public event OnGameFinishLoadingDelegate gameFinishLoadingDelegate;
    public event OnGameStartDelegate gameStartDelegate;
    public event OnGameEndDelegate gameEndDelegate;

    [SerializeField]
    private GameState gameState = GameState.InMenu;
    [SerializeField]
    private bool isGameLoading = false;
    [SerializeField]
    private bool isGamePaused = false;

    public PauseMenu pauseMenu;

    public static GameManager GetInstance()
    {
        return Instance;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Object.Destroy(Instance.gameObject);
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        //define what will happen when game is paused
        //TODO: pause music?
        gamePauseDelegate += pauseMenu.DisplayPauseMenu;

        //define what will happen when game is resumed
        gameResumeDelegate += pauseMenu.HidePauseMenu;

        gameEndDelegate += ResumeGame;
        gameEndDelegate += pauseMenu.DisableMenu;
    }

    /* Game logic */
    public void StartGame()
    {
        Debug.Log("[" + Time.time + "]" + " Start game ...");
        gameState = GameState.StartGame;
        ChangeScene();
    }

    private void GameInit()
    {
        Debug.Log("[" + Time.time + "]" + " Start game initialization...");
        pauseMenu.gameObject.SetActive(true);
        StartCoroutine(GamePauseCheck());
        gameStartDelegate?.Invoke();
        gameState = GameState.InGame;

        Debug.Log("[" + Time.time + "]" + " Game initialization done");
    }

    public void EndGame()
    {
        Debug.Log("[" + Time.time + "]" + " Ending game ...");
        gameState = GameState.FinishGame;
        StopCoroutine(GamePauseCheck());
        gameEndDelegate?.Invoke();
        ChangeToMenuScene();
    }

    private void Reset()
    {
        Debug.Log("[" + Time.time + "]" + " Reset value ...");
    }

    public bool IsInGame()
    {
        return gameState == GameState.InGame;
    }

    public bool IsGameLoading()
    {
        return isGameLoading;
    }

    /* Scene Management */
    public SceneLoading loadingScreen;
    [System.Serializable] public class SceneDictionary : SerializableDictionary<string, string> { };
    public SceneDictionary sceneMap;
    
    
    public string GetCurrentScene()
    {
        return SceneManager.GetActiveScene().name;
    }

    public string GetNextScene()
    {
        return sceneMap.Get(SceneManager.GetActiveScene().name);
    }

    public void ChangeToMenuScene()
    {
        isGameLoading = true;
        gameStartLoadingDelegate?.Invoke();
        //start async operation
        StartCoroutine(LoadAsyncOperation("Menu"));
    }

    public void ChangeScene()
    {
        
        isGameLoading = true;
        gameStartLoadingDelegate?.Invoke();

        Scene scene = SceneManager.GetActiveScene();
        //start async operation
        StartCoroutine(LoadAsyncOperation(sceneMap.Get(scene.name)));
    }

    IEnumerator LoadAsyncOperation(string sceneToLoad)
    {
        Debug.Log("[" + Time.time + "]" + " Start loading scene " + sceneToLoad + " ...");
        //create an async operation
        AsyncOperation gameLevel = SceneManager.LoadSceneAsync(sceneToLoad);

        while (gameLevel.progress < 1)
        {
            //update progress bar
            loadingScreen.UpdateLoadingProgress(gameLevel.progress);
            yield return new WaitForEndOfFrame();
        }

        if (gameState == GameState.StartGame) GameInit();

        gameFinishLoadingDelegate?.Invoke();
        isGameLoading = false;
        Debug.Log("[" + Time.time + "]" + " Finish loading " + sceneToLoad);
    }

    /* Game pause */
    IEnumerator GamePauseCheck()
    {
        while (true)
        {
            if (isGameLoading) yield return new WaitForEndOfFrame();
            if (Keyboard.current.escapeKey.isPressed)
            {
                PauseGame();
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void PauseGame()
    {
        if (isGamePaused) return;
        Debug.Log("[" + Time.time + "]" + " Pause Game");
        gamePauseDelegate?.Invoke();
        isGamePaused = true;
    }

    public void PauseGameCallback()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        if (!isGamePaused) return;
        Debug.Log("[" + Time.time + "]" + " Resume Game");
        Time.timeScale = 1;
        gameResumeDelegate?.Invoke();
        isGamePaused = false;
    }
    
}
