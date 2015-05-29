using UnityEngine;
using System.Collections;

public class CameraLookAt : MonoBehaviour
{
  // Defaults
  private Quaternion defaultRotation;

  private float rotateFactor = 150f;
  private float translateFactor = 8f;
  private float default_zOffset = 5f;
  private float zOffset;
  public GameObject parent; // La camara sigue al objeto

  void Start()
  {
    defaultRotation = transform.rotation;
    Reset();
  }

  public void Reset()
  {
    //Cursor.lockState = CursorLockMode.Locked;
    //Cursor.visible = false;
    zOffset = default_zOffset;
    if (parent != null)
    {
      transform.position = parent.transform.position - parent.transform.forward * zOffset;
    }
    transform.rotation = defaultRotation;
  }

  public void SetParent(GameObject parent)
  {
    this.parent = parent;
  }

  void Update()
  {
    if (parent == null)
    {
      return;
    }

    transform.position = parent.transform.position - transform.forward * zOffset;
    transform.LookAt(parent.transform);

    // Altura/rotacion vertical de la camara
    //if (Input.GetKey(KeyCode.PageUp) || Input.GetKey(KeyCode.I))
    //{
    //  transform.RotateAround(parent.transform.position, Vector3.right, rotateFactor * Time.deltaTime);
    //}

    //if (Input.GetKey(KeyCode.PageUp) || Input.GetKey(KeyCode.K))
    //{
    //  transform.RotateAround(parent.transform.position, Vector3.left, rotateFactor * Time.deltaTime);
    //}

    // Rotacion horizontal
    if (Input.GetKey(KeyCode.L)) // Derecha
    {
      transform.RotateAround(parent.transform.position, Vector3.down, rotateFactor * Time.deltaTime);
    }

    if (Input.GetKey(KeyCode.J)) // Izquierda
    {
      transform.RotateAround(parent.transform.position, Vector3.up, rotateFactor * Time.deltaTime);
    }

    // Giro con raton
    //float mouseAxis = Input.GetAxis("Mouse X");
    //if (mouseAxis != 0)
    //{
    //  int rotateDir = 0;
    //  if (mouseAxis > 0.8f)
    //  {
    //    rotateDir = 1;
    //  }
    //  else if (mouseAxis < -0.8f)
    //  {
    //    rotateDir = -1;
    //  }
    //  transform.RotateAround(parent.transform.position, Vector3.up * rotateDir, rotateFactor * Time.deltaTime);
    //}

    // Zoom
    if ((Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus)) && zOffset >= 1f)
    {
      zOffset -= translateFactor * Time.deltaTime;
    }
    if ((Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus)) && zOffset <= 6f)
    {
      zOffset += translateFactor * Time.deltaTime;
    }

    if (Input.GetKey(KeyCode.Home))
    {
      Reset();
    }
  }
}
