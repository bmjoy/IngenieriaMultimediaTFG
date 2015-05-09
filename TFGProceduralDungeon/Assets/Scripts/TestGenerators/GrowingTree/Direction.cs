using UnityEngine;

public enum Direction
{
  North,
  East,
  South,
  West
}

public static class DungeonDirections
{

  public const int Count = 4;

  public static Direction RandomValue
  {
    get
    {
      return (Direction)Random.Range(0, Count);
    }
  }

  private static Direction[] opposites = {
		Direction.South,
		Direction.West,
		Direction.North,
		Direction.East
	};

  public static Direction GetOpposite(this Direction direction)
  {
    return opposites[(int)direction];
  }
	
  private static Vector2i[] vectors = {
		new Vector2i(0, 1),
		new Vector2i(1, 0),
		new Vector2i(0, -1),
		new Vector2i(-1, 0)
	};
	
  public static Vector2i ToIntVector2(this Direction direction)
  {
    return vectors[(int)direction];
  }

  private static Quaternion[] rotations = {
		Quaternion.identity,
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, 180f, 0f),
		Quaternion.Euler(0f, 270f, 0f)
	};
	
  public static Quaternion ToRotation(this Direction direction)
  {
    return rotations[(int)direction];
  }
}