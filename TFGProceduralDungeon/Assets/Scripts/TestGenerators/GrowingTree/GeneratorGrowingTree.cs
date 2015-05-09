using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneratorGrowingTree : MonoBehaviour
{

  public Vector2i size;
  public DungeonCell cellPrefab;
  public float generationStepDelay;
  public DungeonPassage passagePrefab;
  public DungeonWall wallPrefab;

  private Coroutine routine;
  private DungeonCell[,] cells;

  public bool ContainsCoordinates(Vector2i coordinate)
  {
    return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
  }

  public DungeonCell GetCell(Vector2i coordinates)
  {
    return cells[coordinates.x, coordinates.z];
  }

  public void Generate(int width, int height)
  {
    Cleanup();
    routine = StartCoroutine(CreateDungeon(width, height));
  }

  public void Cleanup()
  {
    if (routine != null)
    {
      StopCoroutine(routine);
    }
  }

  public IEnumerator CreateDungeon(int width, int height)
  {
    size.x = 20;
    size.z = 20;

    WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
    cells = new DungeonCell[size.x, size.z];
    List<DungeonCell> activeCells = new List<DungeonCell>();
    Vector2i randomCoor = new Vector2i(Random.Range(0, size.x), Random.Range(0, size.z));
    activeCells.Add(CreateCell(randomCoor));
    while (activeCells.Count > 0)
    {
      yield return delay;
      DoNextGenerationStep(activeCells);
    }
  }

  private void DoNextGenerationStep(List<DungeonCell> activeCells)
  {
    int currentIndex = activeCells.Count - 1;
    DungeonCell currentCell = activeCells[currentIndex];
    if (currentCell.IsFullyInitialized)
    {
      activeCells.RemoveAt(currentIndex);
      return;
    }
    Direction direction = currentCell.RandomUninitializedDirection;
    Vector2i coordinates = currentCell.coordinates + direction.ToIntVector2();
    if (ContainsCoordinates(coordinates))
    {
      DungeonCell neighbor = GetCell(coordinates);
      if (neighbor == null)
      {
        neighbor = CreateCell(coordinates);
        CreatePassage(currentCell, neighbor, direction);
        activeCells.Add(neighbor);
      }
      else
      {
        CreateWall(currentCell, neighbor, direction);
      }
    }
    else
    {
      CreateWall(currentCell, null, direction);
    }
  }

  private DungeonCell CreateCell(Vector2i coordinates)
  {
    DungeonCell newCell = Instantiate(cellPrefab) as DungeonCell;
    cells[coordinates.x, coordinates.z] = newCell;
    newCell.coordinates = coordinates;
    newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
    newCell.transform.parent = transform;
    newCell.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0f, coordinates.z - size.z * 0.5f + 0.5f);
    return newCell;
  }

  private void CreatePassage(DungeonCell cell, DungeonCell otherCell, Direction direction)
  {
    DungeonPassage passage = Instantiate(passagePrefab) as DungeonPassage;
    passage.Initialize(cell, otherCell, direction);
    passage = Instantiate(passagePrefab) as DungeonPassage;
    passage.Initialize(otherCell, cell, direction.GetOpposite());
  }

  private void CreateWall(DungeonCell cell, DungeonCell otherCell, Direction direction)
  {
    DungeonWall wall = Instantiate(wallPrefab) as DungeonWall;
    wall.Initialize(cell, otherCell, direction);
    if (otherCell != null)
    {
      wall = Instantiate(wallPrefab) as DungeonWall;
      wall.Initialize(otherCell, cell, direction.GetOpposite());
    }
  }
}