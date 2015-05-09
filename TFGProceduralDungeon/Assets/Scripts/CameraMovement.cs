using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
  private float rotateFactor = 40f;
  private float translateFactor = 2f;
  private float zOffset = 4f;
  private Vector3 defaultPosition;
  private Quaternion defaultRotation;
  public GameObject player; // La camara sigue al jugador

  void Start()
  {
    defaultPosition = transform.position;
    defaultRotation = transform.rotation;
  }

  public void RegisterPlayer(GameObject player)
  {
    this.player = player;
  }
  
  void Update()
  {
    if(player != null)
    {
      transform.position = new Vector3(player.transform.position.x,
                                       transform.position.y,
                                       player.transform.position.z - zOffset);
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

    // Zoom
    if(Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus))
    {
      zOffset += translateFactor * Time.deltaTime;
    }
    if(Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
    {
      zOffset -= translateFactor * Time.deltaTime;
    }

    if(Input.GetKey(KeyCode.Home))
    {
      transform.position = transform.position = new Vector3(player.transform.position.x,
                                                            defaultPosition.y,
                                                            player.transform.position.z - zOffset);
      transform.rotation = defaultRotation;
    }
  }
}
