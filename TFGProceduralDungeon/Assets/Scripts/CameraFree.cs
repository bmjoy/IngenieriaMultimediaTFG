using UnityEngine;
using System.Collections;

public class CameraFree : MonoBehaviour
{
  public float cameraSensitivity = 200;
  public float climbSpeed = 8;
  public float normalMoveSpeed = 20;
  public float slowMoveFactor = 0.25f;
  public float fastMoveFactor = 3;

  private float rotationX = 0.0f;
  private float rotationY = 0.0f;


  void Update()
  {
    if (Input.GetKey(KeyCode.Mouse1))
    {
      rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
      rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
      rotationY = Mathf.Clamp(rotationY, -90, 90);

      transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
      transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
    }

    float moveFactor = 1f;

    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
    {
      moveFactor = fastMoveFactor;
    }
    else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
    {
      moveFactor = slowMoveFactor;
    }

    transform.position += transform.forward * normalMoveSpeed * moveFactor * Input.GetAxis("Vertical") * Time.deltaTime;
    transform.position += transform.right * normalMoveSpeed * moveFactor * Input.GetAxis("Horizontal") * Time.deltaTime;

    if (Input.GetKey(KeyCode.Q))
    {
      transform.position += transform.up * climbSpeed * moveFactor * Time.deltaTime;
    }
    if (Input.GetKey(KeyCode.E))
    {
      transform.position -= transform.up * climbSpeed * moveFactor * Time.deltaTime;
    }
  }
}