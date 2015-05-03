using UnityEngine;
using System.Collections;

public class EnemyBat : MonoBehaviour
{
  private float speed = 0.5f;
  private bool flying = false; // Indica si esta en pleno vuelo
  private Vector3 path; // El punto hacia donde volar
  private Vector3 objective;

  // Update is called once per frame
  void Update()
  {
    // Actualizamos la posicion en vuelo
    if(flying)
    {
      transform.position += path * speed * Time.deltaTime;
      // Si alcaza la posicion donde esta/estaba el jugador
      Vector3 vDistance = transform.position - objective;
      if(vDistance.sqrMagnitude < 0.1f)
      {
        // Invierte el vuelo y se movera hacia arriba
        path = Vector3.Scale(path, new Vector3(1, -1, 1));
      }
    }
  }

  // Prepara la trayectoria de vuelo
  private void PrepareAttack(Vector3 playerPosition)
  {
    flying = true;
    // Calculamos el vector con respecto al jugador
    objective = playerPosition;
    path = playerPosition - transform.position;
    // Orientamos al murcielago en la direccion correcta
    if(playerPosition.x > transform.position.x)
    {
      transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
    }
  }

  void OnCollisionEnter(Collision collision)
  {
    string collisionTag = collision.gameObject.tag;
    if(collisionTag == "Wall")
    {
      // Detiene el vuelo, se engancha en la pared y resetea la direccion
      transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
      flying = false;
    }
  }

  void OnTriggerEnter(Collider collider)
  {
    string collisionTag = collider.gameObject.tag;

    if(collisionTag == "Player" && !flying)
    {
      PrepareAttack(collider.transform.position);
    }
  }
}
