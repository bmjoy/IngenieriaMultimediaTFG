using UnityEngine;
using System.Collections;

public class Player : MovingObject
{
  private Animator animator;
  private float maxWalkSpeed = 2.0f;
  private bool rightDir = true; // Para girar el sprite en la direccion correcta
  private bool startJump = false; // Pasa que el FixedUpdate aplique la fuerza
  private bool onGround = true; // Indica si el jugador esta en le suelo (no saltando o cayendo)
  private GameObject playerCamera; // Camara que sigue al jugador
  private GameObject sprite; // Objecto hijo sprite
  private Rigidbody rigidBody; // Al que aplicar fuerza para salto

  void Start()
  {
    //animator = GetComponent<Animator>();
    playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
    sprite = GameObject.Find("PlayerSprite");
    animator = sprite.GetComponent<Animator>();
    rigidBody = GetComponent<Rigidbody>();
  }

  void Update()
  {
    // Get the horizontal and vertical axis.
    // By default they are mapped to the arrow keys.
    // The value is in the range -1 to 1
    float hAxis = Input.GetAxis("Horizontal");
    float hWalkSpeed = hAxis * maxWalkSpeed;
    float vWalkSpeed = Input.GetAxis("Vertical") * maxWalkSpeed;
    // Move translation along the object's x/z-axis
    transform.Translate(hWalkSpeed * Time.deltaTime, 0, 0);
    transform.Translate(0, 0, vWalkSpeed * Time.deltaTime);


    // Orienta el sprite hacia la camara, pero solo en el eje x
    // esto ademas lo invierte?
    //    Vector3 lookAtPosition = playerCamera.transform.position;
    //    lookAtPosition.y = sprite.transform.position.y;
    //    sprite.transform.LookAt(lookAtPosition);

    // On 0 movement won't flip sprite
    if (hAxis != 0)
    {
      bool goingRight = true;
      if (Input.GetAxis("Horizontal") < 0)
      {
        goingRight = false;
      }

      // Change direction of the sprite
      if (goingRight != rightDir)
      {
        transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
        rightDir = goingRight;
      }
    }

    // SALTO. Solo cuando esta en el suelo
    if (rigidBody.velocity.y == 0)
    {
      onGround = true;
    }
    if (onGround && Input.GetKeyDown(KeyCode.Space))
    {
      // Activamos el flag y la fisica se hace en FixedUpdate
      startJump = true;
      onGround = false;
    }

    // ANIMACIONES
    // Threshold speed for walk/idle animation
    if (!this.animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack01") &&
      !this.animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack02"))
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
    if (Input.GetKeyDown(KeyCode.Z))
    {
      animator.Play("PlayerAttack01");
    }
    else if (Input.GetKeyDown(KeyCode.X))
    {
      animator.Play("PlayerAttack02");
    }
  }

  void FixedUpdate()
  {
    if (startJump)
    {
      startJump = false;
      rigidBody.velocity *= 0;
      rigidBody.AddForce(Vector3.up * 150);
    }
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
