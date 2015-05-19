using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneratorGrowingTree : MonoBehaviour
{

  public Vector2i size;
  public t_DungeonCell cellPrefab;
  public float generationStepDelay;
  public t_DungeonPassage passagePrefab;
  public t_DungeonWall wallPrefab;
  public bool randomIndex = false; // Indica si se selecciona una celda aleatoria en cada paso
  // Quad que muestra visualmente la posicion del generador en cada momento
  private GameObject helper;

  private Coroutine routine;
  private t_DungeonCell[,] cells;


  public bool ContainsCoordinates(Vector2i coordinate)
  {
    return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
  }

  public t_DungeonCell GetCell(Vector2i coordinates)
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
    cells = new t_DungeonCell[size.x, size.z];
    List<t_DungeonCell> activeCells = new List<t_DungeonCell>();
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
  private int GetNextIndex(List<t_DungeonCell> activeCells)
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

  private void DoNextGenerationStep(List<t_DungeonCell> activeCells)
  {
    int currentIndex = GetNextIndex(activeCells);
    UpdateHelper(activeCells[currentIndex].transform.position);

    t_DungeonCell currentCell = activeCells[currentIndex];
    if (currentCell.IsFullyInitialized)
    {
      activeCells.RemoveAt(currentIndex);
      return;
    }
    t_Direction direction = currentCell.RandomUninitializedDirection;
    Vector2i coordinates = currentCell.coordinates + direction.ToIntVector2();
    if (ContainsCoordinates(coordinates))
    {
      t_DungeonCell neighbor = GetCell(coordinates);
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

  private t_DungeonCell CreateCell(Vector2i coordinates)
  {
    t_DungeonCell newCell = Instantiate(cellPrefab) as t_DungeonCell;
    cells[coordinates.x, coordinates.z] = newCell;
    newCell.coordinates = coordinates;
    newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
    newCell.transform.parent = transform;
    newCell.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0f, coordinates.z - size.z * 0.5f + 0.5f);
    return newCell;
  }

  private void CreatePassage(t_DungeonCell cell, t_DungeonCell otherCell, t_Direction direction)
  {
    t_DungeonPassage passage = Instantiate(passagePrefab) as t_DungeonPassage;
    passage.Initialize(cell, otherCell, direction);
    passage = Instantiate(passagePrefab) as t_DungeonPassage;
    passage.Initialize(otherCell, cell, direction.GetOpposite());
  }

  private void CreateWall(t_DungeonCell cell, t_DungeonCell otherCell, t_Direction direction)
  {
    t_DungeonWall wall = Instantiate(wallPrefab) as t_DungeonWall;
    wall.Initialize(cell, otherCell, direction);
    if (otherCell != null)
    {
      wall = Instantiate(wallPrefab) as t_DungeonWall;
      wall.Initialize(otherCell, cell, direction.GetOpposite());
    }
  }
}