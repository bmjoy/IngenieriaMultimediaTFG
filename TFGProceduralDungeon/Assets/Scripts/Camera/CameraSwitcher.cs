using UnityEngine;
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
  public bool locked = false;

  private CameraLookAt cameraLookAt;
  private CameraFree cameraFree;
  private CameraClearVision cameraClearVision;

  // Use this for initialization
  void Start()
  {
    cameraLookAt = GetComponent<CameraLookAt>();
    cameraFree = GetComponent<CameraFree>();
    cameraClearVision = GetComponent<CameraClearVision>();
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.C))
    {
      cameraLookAt.enabled = !cameraLookAt.enabled;
      cameraClearVision.enabled = !cameraClearVision.enabled;
      cameraFree.enabled = !cameraFree.enabled;

      if (cameraLookAt.enabled)
      {
        cameraLookAt.Reset();
      }
    }
  }
}
