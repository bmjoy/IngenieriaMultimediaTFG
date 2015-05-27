using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
  public GameObject heartEmptySprite;
  public GameObject heartFullSprite;

  private GameObject[] healthSprites;
  private GameObject healthPanel;

  void Start()
  {
    healthPanel = GameObject.Find("PanelHearts");
    healthSprites = new GameObject[Player.MAX_HEALTH];

    for (int i = 0; i < Player.MAX_HEALTH; i++)
    {
      healthSprites[i] = Instantiate(heartEmptySprite);
    }

    SetHealthMeter(Player.MAX_HEALTH);
  }

  public void SetHealthMeter(int health)
  {
    RectTransform prt = healthPanel.GetComponent<RectTransform>();
    float startPosition = prt.position.x - 80f;
    float separation = 60f;

    for (int i = 0; i < Player.MAX_HEALTH; i++)
    {
      Destroy(healthSprites[i].gameObject);
      if (i <= health)
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
