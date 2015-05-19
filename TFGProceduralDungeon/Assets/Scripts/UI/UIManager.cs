using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
  private GameObject menuTests;
  private GameObject menuMain;

  private void Start()
  {
    if (Application.loadedLevel == (int)SceneName.MainMenu)
    {
      menuMain = GameObject.Find("MenuMain");
      menuTests = GameObject.Find("MenuTests");
      menuTests.SetActive(false);
    }
  }

  // Eventos de boton
  public void OnNewGame()
  {
    GameManager.instance.LoadScene(SceneName.Intro);
  }

  public void OnSettings()
  {
  }

  public void OnBack()
  {
    GameManager.instance.LoadScene(SceneName.MainMenu);
  }

  /*********** TESTS ***********/
  public void ShowTestsMenu()
  {
    menuMain.SetActive(false);
    menuTests.SetActive(true);
  }

  public void OnTestLevelGeneration()
  {
    GameManager.instance.LoadScene(SceneName.TestLevelGeneration);
  }

  public void OnTestEnemies()
  {
    GameManager.instance.LoadScene(SceneName.TestEnemies);
  }
}
