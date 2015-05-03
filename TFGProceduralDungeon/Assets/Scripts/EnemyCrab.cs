using UnityEngine;
using System.Collections;

public class EnemyCrab : MonoBehaviour
{
  private float speed = 0.8f;
  private bool rightDir = true;
  private Animator animator;
  public bool patrolling = false;

  void Start()
  {
    animator = GetComponent<Animator>();
  }

  void Update()
  {
    if(patrolling)
    {
      if(rightDir)
      {
        transform.Translate(speed * Time.deltaTime, 0, 0);
      }
      else
      {
        transform.Translate(-speed * Time.deltaTime, 0, 0);
      }
      // Evita que la animacion de ataque sea interrumpida
      if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
      {
        animator.SetTrigger("Walk");
      }
    }
  }

  void OnCollisionEnter(Collision collision)
  {
    string cTag = collision.gameObject.tag;
    if(cTag == "Wall")
    {
      rightDir = !rightDir;
    }
    else if(cTag == "Player")
    {
      animator.Play("EnemyCrabAttack");
    }
  }
}
