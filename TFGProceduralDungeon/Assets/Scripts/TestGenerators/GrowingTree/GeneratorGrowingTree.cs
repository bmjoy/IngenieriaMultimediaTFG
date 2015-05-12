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
  public bool randomIndex = false; // Indica si se selecciona una celda aleatoria en cada paso
  // Quad que muestra visualmente la posicion del generador en cada momento
  private GameObject helper;

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

  public void Generate(float delay, bool random)
  {
    Cleanup();
    routine = StartCoroutine(CreateDungeon(delay, random));
  }

  public void Cleanup()
  {
    if (routine != null)
    {
      StopCoroutine(routine);
    }
    if (helper != null)
    {
      Destroy(helper);
    }
  }

  private void UpdateHelper(Vector3 position)
  {
    helper.transform.position = position;
    helper.transform.Translate(0, 0.1f, 0);
  }

  public IEnumerator CreateDungeon(float d, bool r)
  {
    size.x = 10;
    size.z = 10;

    generationStepDelay = d;
    randomIndex = r;

    WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
    cells = new DungeonCell[size.x, size.z];
    List<DungeonCell> activeCells = new List<DungeonCell>();
    Vector2i randomCoor = new Vector2i(Random.Range(0, size.x), Random.Range(0, size.z));
    activeCells.Add(CreateCell(randomCoor));
    // Creamos el helper para ver por donde se va generando
    helper = GameObject.CreatePrimitive(PrimitiveType.Cube);
    helper.transform.localScale = new Vector3(1f, 0.1f, 1f);
    helper.GetComponent<Renderer>().material.color = Color.yellow;

    while (activeCells.Count > 0)
    {
      yield return delay;
      DoNextGenerationStep(activeCells);
    }

    Destroy(helper);
  }

  // Devuelve el indice de la celda del paso actual
  private int GetNextIndex(List<DungeonCell> activeCells)
  {
    // Por defecto se selecciona la celda mas reciente de la lista (Recursive Backtracker)
    int index = activeCells.Count - 1;

    // Selecciona un indice aleatorio (algoritmo de Prim)
    if (randomIndex)
    {
      index = Random.Range(0, index);
    }

    return index;
  }

  private void DoNextGenerationStep(List<DungeonCell> activeCells)
  {
    int currentIndex = GetNextIndex(activeCells);
    UpdateHelper(activeCells[currentIndex].transform.position);

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