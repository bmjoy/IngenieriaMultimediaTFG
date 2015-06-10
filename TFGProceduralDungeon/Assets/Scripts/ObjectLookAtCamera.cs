using UnityEngine;
using System.Collections;

/**
 * Script que hace que el objeto siempre mire hacia la camara.
 * Generalmente se usa para sprites en entornos 3D
 * */
public class ObjectLookAtCamera : MonoBehaviour
{
  private GameObject cameraInstance;

  void Start()
  {
    cameraInstance = Camera.main.gameObject;
  }

  void Update()
  {
    Vector3 rotationDir = transform.position - cameraInstance.transform.position;
    rotationDir.y = 0f;
    Quaternion newRotation = Quaternion.LookRotation(rotationDir, Vector3.up);
    transform.rotation = newRotation;
  }
}
