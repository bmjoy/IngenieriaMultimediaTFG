using UnityEngine;
using System.Collections;

/**
 * Script que hace que el objeto siempre mire hacia la camara.
 * Generalmente se usa para sprites en entornos 3D
 * */
public class SpriteLookAtCamera : MonoBehaviour
{
  private Camera camera;
  void Update()
  {
    if (camera == null)
    {
      camera = Camera.current;
    }
    else
    {
      transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.down,
                          Camera.main.transform.rotation * Vector3.back);
    }
  }
}
