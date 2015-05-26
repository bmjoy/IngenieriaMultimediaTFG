using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour
{
  public GameObject particlesGemsPrefab;
  private bool opened = false;

  private IEnumerator OpenChest()
  {
    opened = true;
    Animation openAnimation = GetComponent<Animation>();
    if (openAnimation != null)
    {
      openAnimation.Play();
    }
    yield return new WaitForSeconds(0.6f);
    Instantiate(particlesGemsPrefab, transform.position, particlesGemsPrefab.transform.rotation);
  }

  void OnCollisionEnter(Collision collision)
  {
    if (opened)
    {
      return;
    }
    if (collision.gameObject.tag == "Player")
    {
      StartCoroutine(OpenChest());
    }
  }
}
