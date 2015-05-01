using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
  private float rotateFactor = 40f;
  private float translateFactor = 2f;
  private Vector3 defaultPosition;
  private Quaternion defaultRotation;
  private GameObject player;

  void Start()
  {
    defaultPosition = transform.position;
    defaultRotation = transform.rotation;
    player = GameObject.Find("Player");
  }
  
  // Update is called once per frame
  void Update()
  {
    // Posicion de la camara con respecto al jugador
    if(player != null)
    {
      transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
    }

    // Altura/rotacion de la camara
    if(Input.GetKey(KeyCode.PageUp) && transform.rotation.eulerAngles.x < 60f)
    {
      transform.Rotate(Vector3.right * rotateFactor * Time.deltaTime);
      transform.Translate(Vector3.up * translateFactor * Time.deltaTime, Space.World);
    }

    if(Input.GetKey(KeyCode.PageDown) && transform.rotation.eulerAngles.x > 10f)
    {
      transform.Rotate(Vector3.left * rotateFactor * Time.deltaTime);
      transform.Translate(Vector3.down * translateFactor * Time.deltaTime, Space.World);
    }

    if(Input.GetKey(KeyCode.Home))
    {
      transform.position = defaultPosition;
      transform.rotation = defaultRotation;
    }
  }

  // Moves the camera to the right room
  public void MoveRight()
  {
    transform.Translate(6, 0, 0, Space.World);
  }

  public void MoveLeft()
  {
    transform.Translate(-6, 0, 0, Space.World);
  }
}
