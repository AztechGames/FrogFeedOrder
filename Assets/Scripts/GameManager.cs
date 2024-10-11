using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    int asyncSceneIndex = 1;
    public bool taptic = true;
    public int TotalFrog;
    
    public enum GAMESTATE { Start, Ingame, Finish, GameOver, }
    GAMESTATE _gamestate;
    public GAMESTATE gamestate
    {
        get => _gamestate;
        set
        {
            _gamestate = value;
            UIManager.Instance.PanelControl();
        }
    }
    
    
    [HideInInspector]public int MaxMove = 1;
    [HideInInspector]public int MoveLeft;
    
    void Start()
    {
        gamestate = GAMESTATE.Start;
        GameStart();
    }
    void Update()
    {
        if (Input.anyKeyDown && gamestate == GAMESTATE.Start)
        {
            TotalFrog = GameObject.FindGameObjectsWithTag("Player").Length;
            gamestate = GAMESTATE.Ingame;
        }
        
        if(MoveLeft <= 0 && gamestate == GAMESTATE.Ingame)
            gamestate = GAMESTATE.GameOver;
        
        if(TotalFrog <= 0 && gamestate == GAMESTATE.Ingame)
            gamestate = GAMESTATE.Finish;
    }
    #region States
    
    void GameStart()
    {
        asyncSceneIndex = PlayerPrefs.GetInt("SaveScene",asyncSceneIndex);
        if(SceneManager.sceneCount < 2)
            SceneManager.LoadSceneAsync(asyncSceneIndex, LoadSceneMode.Additive);
        UIManager.Instance.SetLevel();
    }
    public void RestartButton()
    {
        SceneManager.UnloadSceneAsync(asyncSceneIndex);
        SceneManager.LoadSceneAsync(asyncSceneIndex, LoadSceneMode.Additive);
        gamestate = GAMESTATE.Start;
    }
    
    public void NextLevelButton()
    {
        if (SceneManager.sceneCountInBuildSettings == asyncSceneIndex + 1)
        {
            SceneManager.UnloadSceneAsync(asyncSceneIndex);
            asyncSceneIndex = 1;
            SceneManager.LoadSceneAsync(asyncSceneIndex, LoadSceneMode.Additive);
        }
        else
        {
            if (SceneManager.sceneCount > 1)
            {
                SceneManager.UnloadSceneAsync(asyncSceneIndex);
                asyncSceneIndex += 1;                
            }

            SceneManager.LoadSceneAsync(asyncSceneIndex, LoadSceneMode.Additive);
        }
        UIManager.Instance.SetLevel();
        PlayerPrefs.SetInt("SaveScene",asyncSceneIndex);
        gamestate = GAMESTATE.Start;
    }
    #endregion
}