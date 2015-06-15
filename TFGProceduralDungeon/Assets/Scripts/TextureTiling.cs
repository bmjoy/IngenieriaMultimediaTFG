using UnityEngine;
using System.Collections;

public class TextureTiling : MonoBehaviour {
  void Start() {
    ReTiling();
  }
  public void ReTiling() {
    GetComponent<Renderer>().material.mainTextureScale = new Vector2(transform.localScale.x, transform.localScale.y);
  }
}
