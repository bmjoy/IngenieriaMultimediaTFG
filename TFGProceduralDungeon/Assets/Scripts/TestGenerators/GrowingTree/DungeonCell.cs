using UnityEngine;

public class DungeonCell : MonoBehaviour
{

  public Vector2i coordinates;

  private DungeonCellEdge[] edges = new DungeonCellEdge[DungeonDirections.Count];

  private int initializedEdgeCount;

  public bool IsFullyInitialized
  {
    get
    {
      return initializedEdgeCount == DungeonDirections.Count;
    }
  }

  public Direction RandomUninitializedDirection
  {
    get
    {
      int skips = Random.Range(0, DungeonDirections.Count - initializedEdgeCount);
      for(int i = 0; i < DungeonDirections.Count; i++)
      {
        if(edges[i] == null)
        {
          if(skips == 0)
          {
            return (Direction)i;
          }
          skips -= 1;
        }
      }
      throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
    }
  }

  public DungeonCellEdge GetEdge(Direction direction)
  {
    return edges[(int)direction];
  }

  public void SetEdge(Direction direction, DungeonCellEdge edge)
  {
    edges[(int)direction] = edge;
    initializedEdgeCount += 1;
  }
}