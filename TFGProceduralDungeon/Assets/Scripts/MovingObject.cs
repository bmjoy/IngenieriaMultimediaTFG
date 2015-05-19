using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour
{

  public float moveTime = 0.1f; // Time object takes to move in seconds
  public LayerMask blockingLayer; // Layer to check collision if the space is free

  private BoxCollider boxCollider;
  private Rigidbody rb; // Reference to the rigid body of the moving unit
  private float inverseMoveTime; // To make calculations more efficient

  // Use this for initialization
  protected virtual void Start()
  {
    boxCollider = GetComponent<BoxCollider>();
    rb = GetComponent<Rigidbody>();
    // We keep the reciprocal of moveTime so we will multiply
    // instead of dividing, which if more efficient computational
    inverseMoveTime = 1f / moveTime;
  }

  protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
  {
    Vector2 start = transform.position;
    // end position based on the direction
    Vector2 end = start + new Vector2(xDir, yDir);
    // Be sure doesn't hit own collider
    boxCollider.enabled = false;
    // Cast a line from start to end point checking collision on block layer
    hit = Physics2D.Linecast(start, end, blockingLayer);
    boxCollider.enabled = true;
    // Check hit
    if (hit.transform == null)
    {
      // Able to move
      StartCoroutine(SmoothMovement(end));
      return true;
    }
    // Move unsuccessful
    return false;
  }

  // The parameter T specifies the type of unit to interact with
  // For player is walls, for enemies is player
  protected virtual void AttemptMove<T>(int xDir, int yDir)
      where T : Component
  {
    RaycastHit2D hit;
    bool canMove = Move(xDir, yDir, out hit);

    if (hit.transform == null)
    {
      return;
    }

    T hitComponent = hit.transform.GetComponent<T>();

    // Cannot move, we pass the hit component, the unit blocking the path
    if (!canMove && hitComponent != null)
    {
      OnCantMove(hitComponent);
    }
  }


  // Movimiento suavizado de un punto a otro
  protected IEnumerator SmoothMovement(Vector3 end)
  {
    // square magnitude is computationally cheaper than magnitude
    float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
    // Epsilon is almost zero
    while (sqrRemainingDistance > float.Epsilon)
    {
      // Move the point from position to end
      // using a step based on deltaTime
      Vector3 newPosition = Vector3.MoveTowards(rb.position, end, inverseMoveTime * Time.deltaTime);
      rb.MovePosition(newPosition);
      // Recalculate distance after moving
      sqrRemainingDistance = (transform.position - end).sqrMagnitude;
      // Wait for a frame before reevaluating the condition of the loop
      yield return null;
    }
  }

  // Function that returns void and takes a generic parameter T
  // as well as parameter of type T called Component
  protected abstract void OnCantMove<T>(T Component)
      where T : Component;

}
