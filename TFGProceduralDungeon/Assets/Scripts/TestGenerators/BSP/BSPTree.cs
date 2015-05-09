using UnityEngine;
using System.Collections;

public class BSPTree
{
  // Nodo raiz
  private BSPNode root;

  public BSPTree(GameObject rootArea)
  {
    root = new BSPNode();
    root.SetCube(rootArea);
  }

  public BSPNode Root
  {
    get
    {
      return this.root;
    }
  }
}
