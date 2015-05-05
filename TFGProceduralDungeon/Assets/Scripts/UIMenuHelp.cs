using UnityEngine;
using System.Collections;

public class UIMenuHelp : MonoBehaviour
{
  public GameObject canvasHelp;
  private bool active = false;

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.F1))
    {
      active = !active;
      canvasHelp.SetActive(active);
    }
  }
}
