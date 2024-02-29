using System;
using UnityEngine.SceneManagement;

public class ManagerDeScene
{
    public static ManagerDeScene Instance;
    public static int CurrentSceneIndex;

    public event EventHandler ReloadSceneHandler;
    public event EventHandler UnloadingScene;
    public event EventHandler LoadedScene;
    public void Awake()
    {
        Instance = this;
        CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void Start()
    {
        SCR_EventHelper.TrySendEvent(LoadedScene, this);
    }

    public static void LoadNextScene()
    {
        SCR_EventHelper.TrySendEvent(Instance.UnloadingScene, Instance);
        SceneManager.LoadScene(++CurrentSceneIndex);
    }

    public static void LoadMainMenu()
    {
        SCR_EventHelper.TrySendEvent(Instance.UnloadingScene, Instance);
        SceneManager.LoadScene(0);
    }

    public static void LoadSceneByIndex(int index)
    {
        SCR_EventHelper.TrySendEvent(Instance.UnloadingScene, Instance);
        SceneManager.LoadScene(index);
    }

    public static void RestartCurrentScene()
    {
        SCR_EventHelper.TrySendEvent(Instance.ReloadSceneHandler, Instance);
        SceneManager.LoadScene(CurrentSceneIndex);
    }
}
