using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
  public float levelStartDelay = 2f;
  public static GameManager instance = null;

  void Awake()
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

    initGame();
  }

  private void OnLevelWasLoaded(int index)
  {
    initGame();
  }

  void initGame()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

}
