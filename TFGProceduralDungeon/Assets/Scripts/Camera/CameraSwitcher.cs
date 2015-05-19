using UnityEngine;
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{

  private CameraLookAt cameraLookAt;
  private CameraFree cameraFree;

  // Use this for initialization
  void Start()
  {
    cameraLookAt = GetComponent<CameraLookAt>();
    cameraFree = GetComponent<CameraFree>();
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.C))
    {
      cameraLookAt.enabled = !cameraLookAt.enabled;
      cameraFree.enabled = !cameraFree.enabled;
    }
  }
}
