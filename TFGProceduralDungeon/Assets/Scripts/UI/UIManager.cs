using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
  public GameObject pauseCanvas;

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

  // Update is called once per frame
  void Update()
  {
    if (pauseCanvas != null && (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)))
    {
      TogglePause();
    }
  }

  public void TogglePause()
  {
    pauseCanvas.SetActive(!pauseCanvas.activeSelf);
    GameManager.instance.SetPause(!GameManager.instance.Paused);
  }

  // Eventos de boton
  public void OnNewGame()
  {
    GameManager.instance.LoadScene(SceneName.Intro);
  }

  public void OnSettings()
  {
  }

  public void OnMainMenu()
  {
    Time.timeScale = 1.0f - Time.timeScale;
    GameManager.instance.LoadScene(SceneName.MainMenu);
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
