using UnityEngine;
using System.Collections;

public class Digger : MonoBehaviour
{

  private Vector3 targetPos;
  private DungeonGenerator dungeonGenerator;

  // Use this for initialization
  void Awake()
  {
    dungeonGenerator = GameObject.Find("DungeonGenerator").GetComponent<DungeonGenerator>();
  }

  public void Begin(Vector3 _targetPos)
  {
    targetPos = _targetPos;

    Dig();
  }

  private void UpdateTile()
  {
    dungeonGenerator.SetTile((int)transform.position.x, (int)transform.position.z, 1);
    dungeonGenerator.SetTile((int)transform.position.x + 1, (int)transform.position.z, 1);
    dungeonGenerator.SetTile((int)transform.position.x - 1, (int)transform.position.z, 1);
    dungeonGenerator.SetTile((int)transform.position.x, (int)transform.position.z + 1, 1);
    dungeonGenerator.SetTile((int)transform.position.x, (int)transform.position.z - 1, 1);

    SurroundTilesWithWall((int)transform.position.x + 1, (int)transform.position.z);
    SurroundTilesWithWall((int)transform.position.x - 1, (int)transform.position.z);
    SurroundTilesWithWall((int)transform.position.x, (int)transform.position.z + 1);
    SurroundTilesWithWall((int)transform.position.x, (int)transform.position.z - 1);

  }

  public void Dig()
  {

    while (transform.position.x != targetPos.x)
    {

      if (transform.position.x < targetPos.x)
      {
        transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
      }
      else
      {
        transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
      }

      UpdateTile();
    }

    while (transform.position.z != targetPos.z)
    {
      if (transform.position.z < targetPos.z)
      {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
      }
      else
      {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
      }

      UpdateTile();
    }

    DestroyImmediate(this);
  }

  public void SurroundTilesWithWall(int _x, int _y)
  {
    if (dungeonGenerator.GetGrid().GetTile(_x + 1, _y) == 0)
    {
      dungeonGenerator.SetTile(_x + 1, _y, 2);
    }

    if (dungeonGenerator.GetGrid().GetTile(_x - 1, _y) == 0)
    {
      dungeonGenerator.SetTile(_x - 1, _y, 2);
    }

    if (dungeonGenerator.GetGrid().GetTile(_x, _y + 1) == 0)
    {
      dungeonGenerator.SetTile(_x, _y + 1, 2);
    }

    if (dungeonGenerator.GetGrid().GetTile(_x, _y - 1) == 0)
    {
      dungeonGenerator.SetTile(_x, _y - 1, 2);
    }
  }
}
