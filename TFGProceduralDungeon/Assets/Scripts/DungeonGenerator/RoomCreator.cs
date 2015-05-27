using UnityEngine;
using System.Collections;

public enum RoomType
{
  DEFAULT,
  ENTRANCE, // Habitacion inicial donde comienza el jugador
  EXIT,     // Habitacion final de la mazmorra
  PASSAGE   // Pasillo que une dos habitaciones
}

public class RoomCreator : MonoBehaviour
{
  public GameObject digger;

  public RoomType type;
  // Posicion y dimensiones en enteros. Son unidades de grid
  public Vector2i position;
  public Vector2i size;
  private int roomID;
  private BSPNode parentNode;
  private GameObject sibling;

  // Establece la geometria de la habitacion sobre el grid
  //public Grid Setup(Grid grid)
  //{
  //  transform.position = new Vector3((int)transform.position.x - (transform.localScale.x / 2),
  //                                  transform.position.y,
  //                                  (int)transform.position.z - (transform.localScale.z / 2));

  //  for (int i = (int)transform.position.x; i < (int)transform.position.x + transform.localScale.x; i++)
  //  {
  //    for (int j = (int)transform.position.z; j < (int)transform.position.z + transform.localScale.z; j++)
  //    {
  //      grid.SetTile(i, j, 1);
  //    }
  //  }
  //  for (int i = 0; i < transform.localScale.x + 1; i++)
  //  {
  //    grid.SetTile((int)transform.position.x + i, (int)transform.position.z, 2);
  //    grid.SetTile((int)transform.position.x + i, (int)(transform.position.z + transform.localScale.z), 2);
  //  }
  //  for (int i = 0; i < transform.localScale.z + 1; i++)
  //  {
  //    grid.SetTile((int)transform.position.x, (int)transform.position.z + i, 2);
  //    grid.SetTile((int)(transform.position.x + transform.localScale.x), (int)transform.position.z + i, 2);
  //  }

  //  return grid;
  //}

  public Grid Setup(Grid grid)
  {
    // Pasamos la posicion y escala a unidades de grid
    // La posicion se situa en la esquina superior izquierda, la primera casilla de la habitacion
    this.size = new Vector2i((int)transform.localScale.x, (int)transform.localScale.z);
    this.position = new Vector2i((int)(transform.position.x - (size.x / 2)), (int)(transform.position.z - (size.z / 2)));

    // Obtenemos la posicion sobre el grid
    //Vector2i startPosition = new Vector2i((int)(transform.position.x - (transform.localScale.x / 2)),
    //                                (int)(transform.position.z - (transform.localScale.z / 2)));

    // Desde la posicion inicial recorremos las dimensiones de la habitacion asignando valor al tile
    for (int i = position.x; i < position.x + size.x; i++)
    {
      for (int j = position.z; j < position.z + size.z; j++)
      {
        grid.SetTile(i, j, 1); // FLOOR
      }
    }

    // WALLS
    for (int i = 0; i <= size.x; i++)
    {
      grid.SetTile(position.x + i, position.z, 2); // Arriba
      grid.SetTile(position.x + i, position.z + size.z, 2); // Abajo
    }
    for (int i = 0; i <= size.z; i++)
    {
      grid.SetTile(position.x, position.z + i, 2); // Izquierda
      grid.SetTile(position.x + size.x, position.z + i, 2); // Derecha
    }

    return grid;
  }

  public void SetID(int aID)
  {
    roomID = aID;
  }

  public void SetParentNode(BSPNode aNode)
  {
    parentNode = aNode;
  }

  public void Connect()
  {
    GetSibiling();
    if (sibling != null)
    {
      RoomCreator siblingRoom = sibling.GetComponent<RoomCreator>();

      Vector3 startPos = new Vector3();
      Vector3 endPos = new Vector3();

      if (siblingRoom.position.z + siblingRoom.size.z < position.z)
      {
        startPos = ChooseDoorPoint(0);
        endPos = sibling.GetComponent<RoomCreator>().ChooseDoorPoint(2);
      }
      else if (siblingRoom.position.z > position.z + size.z)
      {
        startPos = ChooseDoorPoint(2);
        endPos = sibling.GetComponent<RoomCreator>().ChooseDoorPoint(1);
      }
      else if (siblingRoom.position.x + siblingRoom.size.x < position.x)
      {
        startPos = ChooseDoorPoint(3);
        endPos = sibling.GetComponent<RoomCreator>().ChooseDoorPoint(1);
      }
      else if (siblingRoom.position.x > position.x + size.x)
      {
        startPos = ChooseDoorPoint(1);
        endPos = sibling.GetComponent<RoomCreator>().ChooseDoorPoint(3);
      }

      GameObject aDigger = (GameObject)Instantiate(digger, startPos, Quaternion.identity);
      aDigger.GetComponent<Digger>().Dig(endPos);

      parentNode = FindRoomlessParent(parentNode);
      if (parentNode != null)
      {
        int aC = Random.Range(0, 2);
        if (aC == 0)
        {
          parentNode.SetRoom(this.gameObject);
        }
        else
        {
          parentNode.SetRoom(sibling.gameObject);
        }
        sibling.GetComponent<RoomCreator>().SetParentNode(parentNode);
      }
    }
  }

  private void GetSibiling()
  {
    if (parentNode.GetParentNode() != null)
    {
      if (parentNode.GetParentNode().GetLeftNode() != parentNode)
      {
        sibling = parentNode.GetParentNode().GetLeftNode().GetRoom();
      }
      else
      {
        sibling = parentNode.GetParentNode().GetRightNode().GetRoom();
      }
    }
  }

  public Vector3 ChooseDoorPoint(int index)
  {
    switch (index)
    {
      case 0:
        return new Vector3((int)(position.x + Random.Range(1, size.x - 2)), transform.position.y, (int)(position.z));
      case 1:
        return new Vector3((int)(position.x + size.x), transform.position.y, (int)(position.z + Random.Range(1, size.z - 2)));
      case 2:
        return new Vector3((int)(position.x + Random.Range(1, size.x - 2)), transform.position.y, (int)(position.z + size.z));
      case 3:
        return new Vector3((int)(position.x + 1), transform.position.y, (int)(position.z + Random.Range(1, size.z - 2)));
      default:
        return new Vector3(0, 0, 0);
    }
  }

  public BSPNode GetParent()
  {
    return parentNode;
  }

  public BSPNode FindRoomlessParent(BSPNode aNode)
  {
    if (aNode != null)
    {
      if (aNode.GetRoom() == null)
      {
        return aNode;
      }
      else
      {
        return FindRoomlessParent(aNode.GetParentNode());
      }
    }

    return null;
  }
}
