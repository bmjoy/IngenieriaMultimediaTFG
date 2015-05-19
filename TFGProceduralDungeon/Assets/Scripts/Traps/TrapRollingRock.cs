using UnityEngine;
using System.Collections;

public class TrapRollingRock : MonoBehaviour
{
  public GameObject rockObject;
  public float rollingForce = 2000f;
  private bool activated = false;

  private void TriggerRock()
  {
    // La roca se lanza en direccion al trigger
    Vector3 direction = transform.position - rockObject.transform.position;
    rockObject.GetComponent<Rigidbody>().AddForce(direction * rollingForce);
  }

  void OnCollisionEnter(Collision collision)
  {
    if (!activated && collision.gameObject.tag == "Player")
    {
      activated = false;
      TriggerRock();
      gameObject.SetActive(false);
    }
  }
}
