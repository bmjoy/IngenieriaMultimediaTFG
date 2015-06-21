using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
  public GameObject heartEmptySprite;
  public GameObject heartFullSprite;

  private GameObject[] healthSprites;
  private GameObject healthPanel;

  private LevelManager levelManager;

  // Puntos
  private int currentPoints; // Para agregar puntos en cada actualizacion
  private int points; // Tope al que deben llegar los puntos
  private Text pointsText;
  // Timer
  private Text timerText;

  void Start()
  {
    levelManager = GameManager.Instance.levelManager;

    healthPanel = GameObject.Find("PanelHearts");
    healthSprites = new GameObject[Player.MAX_HEALTH];

    for(int i = 0; i < Player.MAX_HEALTH; i++)
    {
      healthSprites[i] = Instantiate(heartEmptySprite);
    }

    SetHealthMeter(Player.MAX_HEALTH);

    pointsText = GameObject.Find("TextPoints").GetComponent<Text>();
    timerText = GameObject.Find("TextTimer").GetComponent<Text>();
  }

  void Update()
  {
    // Puntos HUD
    if(currentPoints < points)
    {
      currentPoints++;
      pointsText.text = currentPoints.ToString();
    }
    // Temporizador
    UpdateTimer();
  }

  private void UpdateTimer()
  {
    string minutes = Mathf.Floor(levelManager.timer / 60).ToString("00");
    string seconds = Mathf.Floor(levelManager.timer % 60).ToString("00");
    timerText.text = minutes + ":" + seconds;
  }

  public void OnPointsChanged(int points)
  {
    this.points = points;
  }

  public void SetHealthMeter(int health)
  {
    RectTransform prt = healthPanel.GetComponent<RectTransform>();
    float startPosition = prt.position.x - 50f;
    float separation = 40f;

    for(int i = 0; i < Player.MAX_HEALTH; i++)
    {
      Destroy(healthSprites[i].gameObject);
      if(i <= health - 1)
      {
        GameObject heartObject = Instantiate(heartFullSprite);
        heartObject.transform.SetParent(healthPanel.transform, false); // Esto lo coloca dentro del panel
        RectTransform rt = heartObject.GetComponent<RectTransform>();
        rt.position = new Vector3(startPosition + (separation * i), prt.position.y, prt.position.z);
        healthSprites[i] = heartObject;
      }
      else
      {
        GameObject heartObject = Instantiate(heartEmptySprite);
        heartObject.transform.SetParent(healthPanel.transform, false); // Esto lo coloca dentro del panel
        RectTransform rt = heartObject.GetComponent<RectTransform>();
        rt.position = new Vector3(startPosition + (separation * i), prt.position.y, prt.position.z);
        healthSprites[i] = heartObject;
      }
    }
  }

}
