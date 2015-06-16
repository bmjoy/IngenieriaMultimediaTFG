using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour
{
  public GameObject particlesGemsPrefab;

  private IEnumerator OpenChest()
  {
    Animation openAnimation = GetComponentInChildren<Animation>();
    Debug.Log(openAnimation);
    if (openAnimation != null)
    {
      openAnimation.Play();
    }
    yield return new WaitForSeconds(0.6f);
    Instantiate(particlesGemsPrefab, transform.position, particlesGemsPrefab.transform.rotation);

    Destroy(gameObject.GetComponent<Chest>());
  }

  void OnTriggerEnter(Collider collider)
  {
    if (collider.gameObject.tag == "Damage")
    {
      StartCoroutine(OpenChest());
    }
  }
}
