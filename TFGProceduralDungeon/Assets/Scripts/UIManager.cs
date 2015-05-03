using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
  GameObject menuTests;
  GameObject menuMain;

  private void Start()
  {
    menuMain = GameObject.Find("MenuMain");
    menuTests = GameObject.Find("MenuTests");
    menuTests.SetActive(false);
  }

  public void LoadMainMenu()
  {
    Application.LoadLevel("MainMenu");
  }

  public void LoadSettings()
  {
    Application.LoadLevel("Settings");
  }

  public void LoadCredits()
  {
    Application.LoadLevel("Credits");
  }

  /*********** TESTS ***********/
  public void LoadTests()
  {
    menuMain.SetActive(false);
    menuTests.SetActive(true);
  }

  public void LoadTestGameplay()
  {
    Application.LoadLevel("TestGameplay");
  }

  public void LoadTestEnemies()
  {
    Application.LoadLevel("TestEnemies");
  }

  public void LoadTestLevelGeneration()
  {
    Application.LoadLevel("TestLevelGeneration");
  }
}
