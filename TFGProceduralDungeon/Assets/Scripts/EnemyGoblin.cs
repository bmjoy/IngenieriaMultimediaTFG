using UnityEngine;
using System.Collections;

public class EnemyGoblin : Enemy
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
    if (patrolling)
    {
      if (rightDir)
      {
        transform.Translate(speed * Time.deltaTime, 0, 0);
      }
      else
      {
        transform.Translate(-speed * Time.deltaTime, 0, 0);
      }
      // Evita que la animacion de ataque sea interrumpida
      if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
      {
        animator.SetTrigger("Walk");
      }
    }
  }

  // Recibe daño
  void Damage()
  {
    Vector3 spawnPosition = transform.position;
    spawnPosition.y += 0.3f;
    Instantiate(deathEffectPrefab, spawnPosition, Quaternion.identity);
    Destroy(this.gameObject);
  }

  void OnCollisionEnter(Collision collision)
  {
    string cTag = collision.gameObject.tag;
    if (cTag == "Wall")
    {
      rightDir = !rightDir;
    }
    else if (cTag == "Player")
    {
      animator.Play("EnemyGoblinAttack");
    }
    else if (cTag == "Damage")
    {
      Damage();
    }
  }
}