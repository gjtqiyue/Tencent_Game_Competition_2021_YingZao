using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;
using TMPro;
using UnityEngine.Playables;

[System.Serializable]
public class GameManager : MonoBehaviour
{
    private static GameManager Instance;

    private bool isLoading;

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

    public PauseMenu pauseMenu;

    [SerializeField]
    private GameState gameState = GameState.InMenu;
    private Dictionary<GameProgressState, bool> gameProgress;

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
            Destroy(Instance.gameObject);
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (gameState == GameState.InGame)
        {
            if (Keyboard.current.numpadMinusKey.isPressed)
            {
                gameProgress[GameProgressState.木作] = true;
                gameProgress[GameProgressState.漆作] = true;
                gameProgress[GameProgressState.石作] = true;
            }
        }
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

    private void GameSetUp()
    {
        Debug.Log("[" + Time.time + "]" + " Start game initialization...");
        pauseMenu.gameObject.SetActive(true);
        StartCoroutine(GamePauseCheck());
        gameStartDelegate?.Invoke();
        gameState = GameState.InGame;

        gameProgress = new Dictionary<GameProgressState, bool>();
        //setup game progress tracker
        foreach (GameProgressState state in Enum.GetValues(typeof(GameProgressState)))
        {
            gameProgress.Add(state, false);
        }
        PlayIntro();
        Debug.Log("[" + Time.time + "]" + " Game initialization done");
    }

    public void FinishGame()
    {
        Debug.Log("[" + Time.time + "]" + " Ending game ...");
        StopAllCoroutines();
        gameEndDelegate?.Invoke();
        gameProgress.Clear();
        gameState = GameState.FinishedGame;
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
        return isLoading;
    }

    /* Scene Management */
    public SceneLoading loadingScreen;

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
        isLoading = true;
        gameStartLoadingDelegate?.Invoke();
        //start async operation
        StartCoroutine(LoadAsyncOperation("Menu"));
    }

    public void ChangeScene()
    {
        isLoading = true;
        gameStartLoadingDelegate?.Invoke();

        Scene scene = SceneManager.GetActiveScene();
        //start async operation
        StartCoroutine(LoadAsyncOperation(sceneMap.Get(scene.name)));
    }

    IEnumerator LoadAsyncOperation(string sceneToLoad)
    {
        //Debug.Log("[" + Time.time + "]" + " Start loading scene " + sceneToLoad + " ...");
        //create an async operation
        AsyncOperation gameLevel = SceneManager.LoadSceneAsync(sceneToLoad);

        while (gameLevel.progress < 1)
        {
            //update progress bar
            loadingScreen.UpdateLoadingProgress(gameLevel.progress);
            yield return new WaitForEndOfFrame();
        }

        if (gameState == GameState.StartGame) GameSetUp();

        isLoading = false;
        gameFinishLoadingDelegate?.Invoke();
        //update state
        if (sceneToLoad == "Menu") gameState = GameState.InMenu;
        else gameState = GameState.InGame;
        //Debug.Log("[" + Time.time + "]" + " Finish loading " + sceneToLoad);
    }

    /* Game pause */
    IEnumerator GamePauseCheck()
    {
        while (true)
        {
            if (isLoading || gameState == GameState.InMenu) yield return new WaitForEndOfFrame();
            else if (Keyboard.current.escapeKey.isPressed)
            {
                PauseGame();
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void PauseGame()
    {
        if (gameState == GameState.Paused) return;
        
        //Debug.Log("[" + Time.time + "]" + " Pause Game");
        gamePauseDelegate?.Invoke();
        gameState = GameState.Paused;
    }

    public void PauseGameCallback()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        if (gameState != GameState.Paused) return;
        //Debug.Log("[" + Time.time + "]" + " Resume Game");
        Time.timeScale = 1;
        gameResumeDelegate?.Invoke();
        gameState = GameState.InGame;
    }

    public GameObject GetPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    
    public void FinishMiniWork(GameProgressState progressState, bool state)
    {
        gameProgress[progressState] = state;
        //switch camera
        GameObject.Find("MiniGameCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 0;
        //enable playerinput
        GameObject player = GetPlayer();
        if (player != null) player.GetComponent<CharacterController>().EnableInput();
    }

    public Dictionary<GameProgressState, bool> GetGameProgress()
    {
        return gameProgress;
    }

    public void PlayMiniWorkGame(GameProgressState workType)
    {
        if (gameState != GameState.InGame) return;
        GameObject camera = GameObject.Find("MiniGameCamera");
        GameObject gamePlayPrefab = Resources.Load<GameObject>("Prefabs/Gameplay/" + Enum.GetName(typeof(GameProgressState), workType));
        Instantiate(gamePlayPrefab, new Vector3(camera.transform.position.x, camera.transform.position.y, 0), Quaternion.identity).SetActive(true);
        //switch camera
        camera.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 100;  //set to a big enough number
        //disable playerinput
        GameObject player = GetPlayer();
        if (player != null) player.GetComponent<CharacterController>().DisableInput();
    }

    public void PlayIntro()
    {
        GameObject intro = GameObject.Find("IntroSequence");
        PlayableDirector d = intro.GetComponent<PlayableDirector>();
        if (d == null) Debug.LogError("No PlayableDirector found for intro sequence");
        //Debug.Log("Play Intro");
        d.Play();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
