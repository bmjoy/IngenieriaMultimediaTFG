using UnityEngine;
using System.Collections;

public class TextureTiling : MonoBehaviour
{
  void Start()
  {
    GetComponent<Renderer>().material.mainTextureScale = new Vector2(transform.localScale.x, transform.localScale.y);
  }
}
