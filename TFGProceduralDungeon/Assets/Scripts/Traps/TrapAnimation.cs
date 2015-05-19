using UnityEngine;
using System.Collections;

public class TrapAnimation : MonoBehaviour
{
  public float delayTime;
  public float startDelay;

  void Start()
  {
    StartCoroutine(ActivateTrap());
  }

  private IEnumerator ActivateTrap()
  {
    yield return new WaitForSeconds(startDelay);
    while (true)
    {
      GetComponent<Animation>().Play();
      yield return new WaitForSeconds(delayTime);
    }
  }
}
