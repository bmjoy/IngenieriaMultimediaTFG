using UnityEngine;
using System.Collections;

public class WallTile : MonoBehaviour
{
  public Vector2i coordinates;

  public WallTile(int x, int z)
  {
    coordinates.x = x;
    coordinates.z = z;
  }
}
