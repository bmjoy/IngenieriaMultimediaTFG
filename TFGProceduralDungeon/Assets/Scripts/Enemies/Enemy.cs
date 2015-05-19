using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
  public GameObject deathEffectPrefab;
  public bool patrolling = false;
  public float speed = 1f;

  private bool attacking = false;
  private bool isDead = false;

  private bool rightDir = true;
  private Animator animator;
  private string defaultAnimation;


  void Start()
  {
    defaultAnimation = "Idle";
    animator = GetComponent<Animator>();
    if (patrolling)
    {
      TriggerAnimation("Walk");
      defaultAnimation = "Walk";
    }
  }

  void Update()
  {
    if (!isDead)
    {
      if (patrolling && !attacking)
      {
        if (rightDir)
        {
          transform.Translate(speed * Time.deltaTime, 0, 0);
        }
        else
        {
          transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
      }
    }
    // Animacion muerte
    else
    {
      transform.Rotate(0f, 0f, 720f * Time.deltaTime);
      if (rightDir)
      {
        transform.localScale -= new Vector3(0.1f, 0.1f, 0f);
      }
      else
      {
        transform.localScale -= new Vector3(-0.1f, 0.1f, 0f);
      }

    }
  }

  // Activa una animacion mediante trigger
  private void TriggerAnimation(string animationName)
  {
    if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
    {
      animator.SetTrigger(animationName);
    }
  }

  // Animacion de ataque
  private IEnumerator Attack()
  {
    attacking = true;
    TriggerAnimation("Attack");
    float delay = animator.GetCurrentAnimatorStateInfo(0).length;
    yield return new WaitForSeconds(delay);
    attacking = false;
    TriggerAnimation(defaultAnimation);
  }

  // El enemigo muere
  private IEnumerator Die()
  {
    isDead = true;
    // Efecto
    Vector3 spawnPosition = transform.position;
    spawnPosition.y += 0.3f;
    Instantiate(deathEffectPrefab, spawnPosition, deathEffectPrefab.transform.rotation);
    // Desactivamos el rigiBody para realizar la rotacion sin colisiones
    Destroy(GetComponent<BoxCollider>());
    Destroy(GetComponent<Rigidbody>());

    yield return new WaitForSeconds(1f);
    Destroy(this.gameObject);
  }

  void OnCollisionEnter(Collision collision)
  {
    string cTag = collision.gameObject.tag;
    if (cTag == "Wall")
    {
      rightDir = !rightDir;
      Vector3 scale = transform.localScale;
      scale.x *= -1;
      transform.localScale = scale;
    }
    else if (cTag == "Player") // Ataque
    {
      StartCoroutine(Attack());
    }
    else if (cTag == "Damage") // Recibe daño
    {
      StartCoroutine(Die());
    }
  }
}
