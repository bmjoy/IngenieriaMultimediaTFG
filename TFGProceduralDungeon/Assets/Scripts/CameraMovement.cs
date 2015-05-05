using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
  private float rotateFactor = 40f;
  private float translateFactor = 2f;
  private Vector3 defaultPosition;
  private Quaternion defaultRotation;

  void Start()
  {
    defaultPosition = transform.position;
    defaultRotation = transform.rotation;
  }
  
  void Update()
  {
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
}
