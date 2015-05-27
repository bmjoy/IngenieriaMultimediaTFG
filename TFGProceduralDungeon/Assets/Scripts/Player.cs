using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
  // Se instancia para crear una zona de daño delante del jugador cuando este ataca
  public GameObject playerAttack;
  private GameObject playerAttackInstance;

  // STATS del personaje
  public static int MAX_HEALTH = 3;
  public bool invincible = false;
  private int life;

  private int hitGraceTime = 1; // Segundos invencible despues de ser golpeado
  private float maxWalkSpeed = 3.0f;
  private float maxRunSpeed = 5.0f;
  private float jumpForce = 200f;
  private bool running = false;
  private bool attacking = false;
  private float direction = -1f; // 1 derecha, -1 izquierda (en eje x)
  private bool rightDir = false; // Para girar el sprite en la direccion correcta
  private bool startJump = false; // Pasa que el FixedUpdate aplique la fuerza
  private bool onGround = true; // Indica si el jugador esta en le suelo y se puede realizar un salto

  private SpriteRenderer spriteRenderer; // Para aplicar efectos directamente
  private Animator animator;
  private Rigidbody rigidBody; // Al que aplicar fuerza para salto

  private Camera cameraMain;
  private CameraShake cameraShaker;

  // HUD
  private Hud hud;

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
    cameraMain = Camera.main;
    cameraShaker = Camera.main.GetComponent<CameraShake>();
    hud = gameObject.GetComponent<Hud>();
  }

  void Update()
  {
    // Input
    // Cuando esta atacando no se puede mover
    if (attacking)
    {
      return;
    }

    // Si se pulsa Shift la velocidad cambia
    if (Input.GetKeyDown(KeyCode.LeftShift))
    {
      running = true;
    }
    if (Input.GetKeyUp(KeyCode.LeftShift))
    {
      running = false;
    }

    float speed = maxWalkSpeed;
    if (running)
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

    // On 0 movement won't flip sprite
    if (hAxis != 0)
    {
      bool goingRight = false;
      direction = -1f;
      if (Input.GetAxis("Horizontal") > 0)
      {
        goingRight = true;
        direction = 1f;
      }

      // Change direction of the sprite
      if (goingRight != rightDir)
      {
        transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
        rightDir = goingRight;
      }
    }

    // SALTO. Solo cuando esta en el suelo
    if (onGround && (Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Mouse1)))
    {
      // Activamos el flag y la fisica se hace en FixedUpdate
      startJump = true;
      onGround = false;
    }

    // ANIMACIONES
    // Threshold speed for walk/idle animation
    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack"))
    {
      float tSpeed = maxWalkSpeed * 0.5f;
      if (Mathf.Abs(vWalkSpeed) > tSpeed || Mathf.Abs(hWalkSpeed) > tSpeed)
      {
        animator.Play("PlayerWalk");
      }
      else
      {
        animator.Play("PlayerIdle");
      }
    }

    // Attack
    if (Input.GetKeyDown(KeyCode.Z) || Input.GetKey(KeyCode.Mouse0))
    {
      animator.Play("PlayerAttack");
      if (!attacking)
      {
        StartCoroutine(Attack());
      }
    }

    if (playerAttackInstance != null)
    {
      playerAttackInstance.transform.position = new Vector3(transform.position.x + (direction * 0.4f), transform.position.y, transform.position.z);
    }
  }

  private IEnumerator Attack()
  {
    attacking = true;
    yield return new WaitForSeconds(0.4f);
    // Instanciamos la caja de daño al lado del Player
    Vector3 position;
    position = new Vector3(transform.position.x + (direction * 0.6f), transform.position.y, transform.position.z);
    playerAttackInstance = (GameObject)Instantiate(playerAttack, position, Quaternion.identity);
    playerAttackInstance.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1);
    yield return new WaitForSeconds(0.5f);
    Destroy(playerAttackInstance);
    attacking = false;
  }

  void FixedUpdate()
  {
    if (startJump)
    {
      startJump = false;
      rigidBody.velocity *= 0;
      rigidBody.AddForce(Vector3.up * jumpForce);
    }
  }

  void OnCollisionEnter(Collision collision)
  {
    onGround = true;

    if (collision.gameObject.tag == "Enemy" && !invincible)
    {
      cameraShaker.Shake(0.4f);
      life--;
      hud.SetHealthMeter(life);
      if (life <= 0) // Game Over
      {
        Debug.Log("Game Over!");
        life = MAX_HEALTH;
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
