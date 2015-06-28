using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
  public GameObject pauseCanvas;

  private GameObject menuTests;
  private GameObject menuMain;

  private bool isCursorLocked = false;

  private void Start()
  {
    if(Application.loadedLevel == (int)SceneName.MainMenu)
    {
      LockMouse(false);
      menuMain = GameObject.Find("MenuMain");
      menuTests = GameObject.Find("MenuTests");
      menuTests.SetActive(false);
    }
    else if(Application.loadedLevel == (int)SceneName.DungeonLevel)
    {
      LockMouse(true);
    }
  }

  // Update is called once per frame
  void Update()
  {
    if(pauseCanvas != null && (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)))
    {
      TogglePause();
    }
    // Hay un bug en Unity 5 y el bloqueo no se queda fijo
    // Lo arreglamos haciendolo en cada actualizacion
    if(isCursorLocked)
    {
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
    }
    else
    {
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
    }
  }

  // Activa/desactiva la pausa
  public void TogglePause()
  {
    pauseCanvas.SetActive(!pauseCanvas.activeSelf);
    GameManager.Instance.SetPause(!GameManager.Instance.Paused);
    LockMouse(!GameManager.Instance.Paused);
  }

  public void LockMouse(bool locked)
  {
    isCursorLocked = locked;
  }

  /*********** Eventos de boton ***********/
  public void OnNewGame()
  {
    GameManager.Instance.LoadScene((int)SceneName.Intro);
  }

  public void OnMainMenu()
  {
    Time.timeScale = 1.0f - Time.timeScale;
    GameManager.Instance.LoadScene((int)SceneName.MainMenu);
  }

  public void OnBack()
  {
    GameManager.Instance.LoadScene((int)SceneName.MainMenu);
  }

  /*********** Otros eventos ***********/
  // Muestra la pantalla de fin de mazmorra
  public void OnLevelFinish()
  {
    GameManager.Instance.SetPause(true);
    LockMouse(false);
    GameObject.Find("CanvasEndScreen").GetComponent<Canvas>().enabled = true;
  }

  /*********** TESTS ***********/
  public void ShowTestsMenu()
  {
    menuMain.SetActive(false);
    menuTests.SetActive(true);
  }

  public void OnTestLevelGeneration()
  {
    GameManager.Instance.LoadScene((int)SceneName.TestLevelGeneration);
  }

  public void OnTestEnemies()
  {
    GameManager.Instance.LoadScene((int)SceneName.TestEnemies);
  }
}
