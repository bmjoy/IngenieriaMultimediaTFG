using UnityEngine;

public abstract class DungeonCellEdge : MonoBehaviour
{
  public DungeonCell cell, otherCell;

  public Direction direction;

  public void Initialize(DungeonCell cell, DungeonCell otherCell, Direction direction)
  {
    this.cell = cell;
    this.otherCell = otherCell;
    this.direction = direction;
    cell.SetEdge(direction, this);
    transform.parent = cell.transform;
    transform.localPosition = Vector3.zero;
    transform.localRotation = direction.ToRotation();
  }
}