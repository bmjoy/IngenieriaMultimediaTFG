﻿using UnityEngine;
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
  DungeonLevel,
  Ending,
  // Tests
  TestLevelGeneration,
  TestEnemies,
  TestTraps
};

public class GameManager : Singleton<GameManager>
{
  public GameObject prefObjectManager;
  public ObjectManager objectManager;

  public GameObject prefDungeonGenerator;
  public DungeonGenerator dungeonGenerator;

  [HideInInspector]
  public Player player; // Referencia al player para que otras clases accedan

  private float timeScale = 1.0f;

  private bool paused = false;
  public bool Paused
  {
    get { return paused; }
    set { paused = value; }
  }

  protected GameManager() { } // guarantee this will be always a singleton only - can't use the constructor!

  private void Start()
  {
    LoadScene(Application.loadedLevel);
  }

  private void Cleanup()
  {
    if (dungeonGenerator != null)
    {
      Destroy(dungeonGenerator.gameObject);
    }
    if (objectManager != null)
    {
      Destroy(objectManager.gameObject);
    }
  }

  public void Init()
  {
    player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    objectManager = Instantiate(prefObjectManager).GetComponent<ObjectManager>();
    objectManager.transform.parent = this.transform;
    dungeonGenerator = Instantiate(prefDungeonGenerator).GetComponent<DungeonGenerator>();
    dungeonGenerator.transform.parent = this.transform;
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

  public void LoadScene(int sceneIndex)
  {
    Application.LoadLevel(sceneIndex);
  }

  private void OnLevelWasLoaded(int index)
  {
    Cleanup();
    switch (index)
    {
      case (int)SceneName.DungeonLevel:
        Init();
        dungeonGenerator.GenerateDungeon();
        break;
      default:
        Init();
        break;
    }
  }
}
