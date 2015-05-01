using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugInfo : MonoBehaviour
{
  float fpsCount;
  bool active = true;
  GameObject goDebugInfo = null;
  Text fpsText;

  void Awake()
  {
    goDebugInfo = GameObject.Find("DebugInfo");
    fpsText = GameObject.Find("FpsCounter").GetComponent<Text>();
  }

  IEnumerator Start()
  {
    while (true)
    {
      if (Time.timeScale == 1)
      {
        yield return new WaitForSeconds(0.1f);
        fpsCount = (1 / Time.deltaTime);
        fpsText.text = "FPS :" + (Mathf.Round(fpsCount));
      }
      else
      {
        fpsText.text = "Pause";
      }
      yield return new WaitForSeconds(0.5f);
    }
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.F1))
    {
      active = !active;
      goDebugInfo.SetActive(active);
    }
  }
}
