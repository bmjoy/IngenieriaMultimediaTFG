using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// Lista de nombre de escenas ordenadas segun su indice en la build
// Hay que asegurarse que estan bien ordenadas cuando se modifique algo en la build
public enum SceneName
{
  Splash = 0,
  MainMenu,
  Intro,
  MapScreen,
  DungeonLevel,
  Ending,
  // Tests
  TestLevelGeneration,
  TestEnemies
};

public class GameManager : MonoBehaviour
{
  public static GameManager instance = null;

  private float levelStartDelay = 2f;
  private float timeScale = 1.0f;

  private bool paused = false;
  public bool Paused
  {
    get { return paused; }
    set { paused = value; }
  }

  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
    else if (instance != this)
    {
      Destroy(gameObject);
    }
    DontDestroyOnLoad(gameObject);
  }

  public void SetPause(bool pause)
  {
    Time.timeScale = timeScale;
    paused = pause;
    if (pause)
    {
      Time.timeScale = 0f;
    }
  }

  // Carga una escena por nombre
  public void LoadScene(SceneName sceneName)
  {
    Application.LoadLevel((int)sceneName);
  }

  private void OnLevelWasLoaded(int index)
  {
  }
}
