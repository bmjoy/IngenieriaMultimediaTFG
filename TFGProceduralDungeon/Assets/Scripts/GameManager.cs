using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum ItemPoints
{
  COIN = 5,
  CHEST = 50
}

// Lista de nombre de escenas ordenadas segun su indice en la build
// Hay que asegurarse que estan bien ordenadas cuando se modifique algo en la build
public enum SceneName
{
  Preload = 0,
  Splash,
  MainMenu,
  Intro,
  DungeonLevel,
  Ending,
  // Tests
  TestLevelGeneration,
  TestEnemies,
  TestTraps
}

public class GameManager : MonoBehaviour
{
  public GameObject prefDebugTools;
  [HideInInspector]
  public GameObject
    debugTools;
//  public GameObject prefObjectManager;
  [HideInInspector]
  public ObjectManager
    objectManager;
//  public GameObject prefLevelManager;
  [HideInInspector]
  public LevelManager
    levelManager;

  [HideInInspector]
  public Player
    player; // Referencia al player para que otras clases accedan

  public int level = 0;

  private float timeScale = 1.0f;
  private bool paused = false;

  private static GameManager _instance;
  
  public static GameManager Instance
  {
    get
    {
      if(_instance == null)
      {
        _instance = GameObject.FindObjectOfType<GameManager>();
        DontDestroyOnLoad(_instance.gameObject);
      }
      
      return _instance;
    }
  }
  
  void Awake()
  {
    if(_instance == null)
    {
      _instance = this;
      DontDestroyOnLoad(this);
    }
    else
    {
      if(this != _instance)
      {
        DestroyImmediate(this.gameObject);
      }
    }
  }

  public bool Paused
  {
    get { return paused; }
    set { paused = value; }
  }

  private void Cleanup()
  {
    //levelManager.Cleanup();
//    if(levelManager != null)
//    {
//      levelManager.Cleanup();
//    }
//    if(objectManager != null)
//    {
//      Destroy(objectManager.gameObject);
//      objectManager = null;
//    }
    if(debugTools != null)
    {
      Destroy(debugTools);
      objectManager = null;
    }
  }

  public void InitLevel()
  {
    // DebugTools
    debugTools = Instantiate(prefDebugTools);
    debugTools.name = "DebugTools";
    // Player
    player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    objectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
    levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    levelManager.Init();
//    // Player
//    player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
//    // Object Manager
//    objectManager = Instantiate(prefObjectManager).GetComponent<ObjectManager>();
//    objectManager.transform.parent = this.transform;
//    // LevelManager
//    if(levelManager == null)
//    {
//      levelManager = Instantiate(prefLevelManager).GetComponent<LevelManager>();
//      levelManager.transform.parent = this.transform;
//    }
//    levelManager.Init();

  }

  public void SetPause(bool pause)
  {
    Time.timeScale = timeScale;
    paused = pause;
    if(pause)
    {
      Time.timeScale = 0f;
    }
  }

  // Carga una escena por indice
  public void LoadScene(int sceneIndex)
  {
    Cleanup();
    StartCoroutine(LoadOnKey(sceneIndex));
    //Application.LoadLevel(sceneIndex);
  }

  private IEnumerator LoadOnKey(int sceneIndex)
  {
    while(!Input.GetKeyDown(KeyCode.K))
    {
      yield return null;

    }
    Application.LoadLevel(sceneIndex);
  }

  private void OnLevelWasLoaded(int index)
  {
    switch(index)
    {
      case (int)SceneName.DungeonLevel:
        SetPause(false);
        InitLevel();
        break;
    }
  }
}
