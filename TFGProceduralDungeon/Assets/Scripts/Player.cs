using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
  // Se instancia para crear una zona de daño delante del jugador cuando este ataca
  public GameObject playerAttack;
  private GameObject playerAttackInstance;

  // STATS del personaje
  public int life = 3;
  public bool invincible = false;

  private int hitGraceTime = 1; // Segundos invencible despues de ser golpeado
  private float maxWalkSpeed = 2.0f;
  private float maxRunSpeed = 3.0f;
  private bool running = false;
  private bool attacking = false;
  private float direction = 1; // 1 derecha, -1 izquierda (en eje x)
  private bool rightDir = true; // Para girar el sprite en la direccion correcta
  private bool startJump = false; // Pasa que el FixedUpdate aplique la fuerza
  private bool onGround = true; // Indica si el jugador esta en le suelo (no saltando o cayendo)

  private SpriteRenderer spriteRenderer; // Para aplicar efectos directamente
  private Animator animator;
  private Rigidbody rigidBody; // Al que aplicar fuerza para salto

  public bool Attacking
  {
    get
    {
      return attacking;
    }
  }

  void Start()
  {
    GameObject sprite = GameObject.Find("PlayerSprite");
    spriteRenderer = sprite.GetComponent<SpriteRenderer>();
    animator = sprite.GetComponent<Animator>();
    rigidBody = GetComponent<Rigidbody>();
  }

  void Update()
  {
    // Si se pulsa Shift la velocidad cambia
    if(Input.GetKeyDown(KeyCode.LeftShift))
    {
      running = true;
    }
    if(Input.GetKeyUp(KeyCode.LeftShift))
    {
      running = false;
    }

    float speed = maxWalkSpeed;
    if(running)
    {
      speed = maxRunSpeed;
    }
    // Get the horizontal and vertical axis.
    // By default they are mapped to the arrow keys.
    // The value is in the range -1 to 1
    float hAxis = Input.GetAxis("Horizontal");
    float hWalkSpeed = hAxis * speed;
    float vWalkSpeed = Input.GetAxis("Vertical") * speed;
    // Move translation along the object's x/z-axis
    transform.Translate(hWalkSpeed * Time.deltaTime, 0, 0);
    transform.Translate(0, 0, vWalkSpeed * Time.deltaTime);

    // Orienta el sprite hacia la camara, pero solo en el eje x
    // esto ademas lo invierte?
    //    Vector3 lookAtPosition = playerCamera.transform.position;
    //    lookAtPosition.y = sprite.transform.position.y;
    //    sprite.transform.LookAt(lookAtPosition);

    // On 0 movement won't flip sprite
    if(hAxis != 0)
    {
      bool goingRight = true;
      direction = 1f;
      if(Input.GetAxis("Horizontal") < 0)
      {
        goingRight = false;
        direction = -1f;
      }

      // Change direction of the sprite
      if(goingRight != rightDir)
      {
        transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
        rightDir = goingRight;
      
      }
    }

    // SALTO. Solo cuando esta en el suelo
    if(rigidBody.velocity.y == 0)
    {
      onGround = true;
    }
    if(onGround && Input.GetKeyDown(KeyCode.Space))
    {
      // Activamos el flag y la fisica se hace en FixedUpdate
      startJump = true;
      onGround = false;
    }

    // ANIMACIONES
    // Threshold speed for walk/idle animation
    if(!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack01") &&
      !animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack02"))
    {
      float tSpeed = maxWalkSpeed * 0.5f;
      if(Mathf.Abs(vWalkSpeed) > tSpeed || Mathf.Abs(hWalkSpeed) > tSpeed)
      {
        animator.Play("PlayerWalk");
      }
      else
      {
        animator.Play("PlayerIdle");
      }
    }

    // Attack
    if(Input.GetKeyDown(KeyCode.Z))
    {
      animator.Play("PlayerAttack01");
      if(!attacking)
      {
        StartCoroutine(Attack());
      }

    }
    else if(Input.GetKeyDown(KeyCode.X))
    {
      animator.Play("PlayerAttack02");
      if(!attacking)
      {
        StartCoroutine(Attack());
      }
    }

    if(playerAttackInstance != null)
    {
      playerAttackInstance.transform.position = new Vector3(transform.position.x + (direction * 0.4f), transform.position.y, transform.position.z);
    }
  }

  private IEnumerator Attack()
  {
    attacking = true;
    // Instanciamos la caja de daño al lado del Player
    Vector3 position;
    position = new Vector3(transform.position.x + (direction * 0.4f), transform.position.y, transform.position.z);
    playerAttackInstance = (GameObject)Instantiate(playerAttack, position, Quaternion.identity);
    yield return new WaitForSeconds(0.3f);
    Destroy(playerAttackInstance);
    attacking = false;
  }

  void FixedUpdate()
  {
    if(startJump)
    {
      startJump = false;
      rigidBody.velocity *= 0;
      rigidBody.AddForce(Vector3.up * 150);
    }
  }

  void OnCollisionEnter(Collision collision)
  {
    if(collision.gameObject.tag == "Enemy" && !invincible)
    {
      life--;
      if(life <= 0) // Game Over
      {
        Debug.Log("Game Over!");
        life = 3;
      }
      else // Tiempo de gracia invencible
      {
        StartCoroutine(OnPlayerDamaged());
      }
    }
  }

  // Alterna el SpriteRenderer para producir efecto
  void BlinkSprite()
  {
    spriteRenderer.enabled = !spriteRenderer.enabled;
  }

  // Espera un tiempo de gracia durante el cual el 
  IEnumerator OnPlayerDamaged()
  {
    invincible = true;
    // Activa el parpadeo del sprite durante el tiempo de gracia
    // Parpadeo de sprite cuando es invencible
    InvokeRepeating("BlinkSprite", 0f, 0.1f);
    yield return new WaitForSeconds(hitGraceTime);
    // Restaura
    CancelInvoke(); // ATENCION: Detiene todos los InvokeRepeating
    spriteRenderer.enabled = !spriteRenderer.enabled;
    invincible = false;
  }

  // Uso trigger para que no interactue fisicamente con la puerta
  // pero se detecte la colision
  void OnTriggerExit(Collider collider)
  {
    // El jugador pasa por una puerta, comprobamos su direccion
    // para mover la camara en la direccion correcta
    //if (collider.gameObject.tag == "Door")
    //{
    //  CameraMovement cameraMove = playerCamera.GetComponent<CameraMovement>();
    //  if (rightDir)
    //  {
    //    cameraMove.MoveRight();
    //  }
    //  else
    //  {
    //    cameraMove.MoveLeft();
    //  }
      
    //}
  }
}
