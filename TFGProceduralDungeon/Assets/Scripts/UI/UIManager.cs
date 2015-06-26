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
      menuMain = GameObject.Find("MenuMain");
      menuTests = GameObject.Find("MenuTests");
      menuTests.SetActive(false);
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
      Cursor.lockState = CursorLockMode.Confined;
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

  // Eventos de boton
  public void OnNewGame()
  {
    GameManager.Instance.LoadScene(SceneName.Intro);
  }

  public void OnSettings()
  {
  }

  public void OnMainMenu()
  {
    Time.timeScale = 1.0f - Time.timeScale;
    GameManager.Instance.LoadScene(SceneName.MainMenu);
  }

  public void OnBack()
  {
    GameManager.Instance.LoadScene(SceneName.MainMenu);
  }

  /*********** TESTS ***********/
  public void ShowTestsMenu()
  {
    menuMain.SetActive(false);
    menuTests.SetActive(true);
  }

  public void OnTestLevelGeneration()
  {
    GameManager.Instance.LoadScene(SceneName.TestLevelGeneration);
  }

  public void OnTestEnemies()
  {
    GameManager.Instance.LoadScene(SceneName.TestEnemies);
  }
}
