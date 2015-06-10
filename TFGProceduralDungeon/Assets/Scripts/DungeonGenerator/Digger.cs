using UnityEngine;
using System.Collections;

public class Digger : MonoBehaviour
{
  private DungeonGenerator generator;


  // Crear suelo alrededor de la posicion actual y la rodea con paredes
  private void UpdateTile()
  {
    generator = GameObject.Find("DungeonGenerator").GetComponent<DungeonGenerator>();
    // Inicializa los tiles del pasillo
    generator.SetTile((int)transform.position.x, (int)transform.position.z, 1);
    generator.SetTile((int)transform.position.x + 1, (int)transform.position.z, 1);
    generator.SetTile((int)transform.position.x - 1, (int)transform.position.z, 1);
    generator.SetTile((int)transform.position.x, (int)transform.position.z + 1, 1);
    generator.SetTile((int)transform.position.x, (int)transform.position.z - 1, 1);
    // Rodea los tiles con pared
    SurroundTilesWithWall((int)transform.position.x + 1, (int)transform.position.z);
    SurroundTilesWithWall((int)transform.position.x - 1, (int)transform.position.z);
    SurroundTilesWithWall((int)transform.position.x, (int)transform.position.z + 1);
    SurroundTilesWithWall((int)transform.position.x, (int)transform.position.z - 1);
  }

  // Avanza en x,z para actualiza cada tile del pasillo a cavar
  public void Dig(Vector3 targetPos)
  {
    // Avance en eje x
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
    // Avance en eje z
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
    Destroy(this.gameObject);
  }

  // Crea un tile de tipo pared si este esta vacio
  public void SurroundTilesWithWall(int x, int y)
  {
    Grid grid = generator.GetGrid();

    if (grid.GetTile(x + 1, y) == 0) // Derecha
    {
      generator.SetTile(x + 1, y, 2);
    }
    if (grid.GetTile(x - 1, y) == 0) // Izquierda
    {
      generator.SetTile(x - 1, y, 2);
    }
    if (grid.GetTile(x, y + 1) == 0) // Superior
    {
      generator.SetTile(x, y + 1, 2);
    }
    if (grid.GetTile(x, y - 1) == 0) // Inferior
    {
      generator.SetTile(x, y - 1, 2);
    }
  }
}
